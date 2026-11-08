using CarritoApi.Data;
using CarritoApi.DTOs.Carrito;
using CarritoApi.Entities;
using CarritoApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CarritoApi.Services;

public class CarritoService : ICarritoService
{
    private readonly AppDbContext _db;
    private readonly IDescuentoService _descuento;

    public CarritoService(AppDbContext db, IDescuentoService descuento)
    {
        _db = db;
        _descuento = descuento;
    }

    public async Task<CarritoDto> ObtenerAsync(int usuarioId, CancellationToken ct = default)
    {
        var carrito = await CargarCarritoAsync(usuarioId, ct);
        return ADto(carrito);
    }

    public async Task<CarritoDto> AgregarProductoAsync(int usuarioId, AgregarProductoRequest request, CancellationToken ct = default)
    {
        var producto = await _db.Productos.AsNoTracking()
                           .FirstOrDefaultAsync(p => p.Id == request.ProductoId && p.Estado, ct)
                       ?? throw new NoEncontradoException($"No existe un producto con id {request.ProductoId}.");

        var carrito = await CargarCarritoAsync(usuarioId, ct) ?? CrearCarrito(usuarioId);
        var detalle = carrito.Detalles.FirstOrDefault(d => d.ProductoId == request.ProductoId);
        var cantidadEnCarrito = detalle?.Cantidad ?? 0;

        // Valida contra lo que ya hay en el carrito + lo que se quiere agregar.
        if (cantidadEnCarrito + request.Cantidad > producto.Stock)
        {
            throw new StockInsuficienteException(producto.NombreProducto, producto.Stock);
        }

        if (detalle is null)
        {
            carrito.Detalles.Add(new CarritoCompraDetalle { ProductoId = producto.Id, Cantidad = request.Cantidad });
        }
        else
        {
            detalle.Cantidad += request.Cantidad;
        }

        carrito.FechaModificacion = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return await ObtenerAsync(usuarioId, ct);
    }

    public async Task<CarritoDto> ActualizarCantidadAsync(int usuarioId, int productoId, int cantidad, CancellationToken ct = default)
    {
        var carrito = await CargarCarritoAsync(usuarioId, ct);
        var detalle = carrito?.Detalles.FirstOrDefault(d => d.ProductoId == productoId)
                      ?? throw new NoEncontradoException("El producto no está en el carrito.");

        if (!detalle.Producto.Estado)
            throw new ConflictoException("Producto no disponible", $"'{detalle.Producto.NombreProducto}' ya no está disponible.");

        if (cantidad > detalle.Producto.Stock)
            throw new StockInsuficienteException(detalle.Producto.NombreProducto, detalle.Producto.Stock);

        detalle.Cantidad = cantidad;
        carrito!.FechaModificacion = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return ADto(carrito);
    }

    public async Task<CarritoDto> EliminarProductoAsync(int usuarioId, int productoId, CancellationToken ct = default)
    {
        var carrito = await CargarCarritoAsync(usuarioId, ct);
        var detalle = carrito?.Detalles.FirstOrDefault(d => d.ProductoId == productoId)
                      ?? throw new NoEncontradoException("El producto no está en el carrito.");

        carrito!.Detalles.Remove(detalle);
        _db.CarritoCompraDetalles.Remove(detalle);
        carrito.FechaModificacion = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return ADto(carrito);
    }

    public async Task VaciarAsync(int usuarioId, CancellationToken ct = default)
    {
        var carrito = await CargarCarritoAsync(usuarioId, ct);
        if (carrito is null || carrito.Detalles.Count == 0) return;

        _db.CarritoCompraDetalles.RemoveRange(carrito.Detalles.ToList());
        carrito.FechaModificacion = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    // ---------- auxiliares ----------

    private Task<CarritoCompra?> CargarCarritoAsync(int usuarioId, CancellationToken ct) =>
        _db.CarritosCompra
           .Include(c => c.Detalles)
               .ThenInclude(d => d.Producto)
                   .ThenInclude(p => p.Categoria)
           .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId, ct);

    private CarritoCompra CrearCarrito(int usuarioId)
    {
        var ahora = DateTime.UtcNow;
        var carrito = new CarritoCompra { UsuarioId = usuarioId, FechaCreacion = ahora, FechaModificacion = ahora };
        _db.CarritosCompra.Add(carrito);
        return carrito;
    }

    private CarritoDto ADto(CarritoCompra? carrito)
    {
        if (carrito is null || carrito.Detalles.Count == 0)
        {
            return new CarritoDto();
        }

        var detalles = carrito.Detalles
            .OrderBy(d => d.Producto.NombreProducto)
            .Select(d => new CarritoDetalleDto
            {
                ProductoId = d.ProductoId,
                Codigo = d.Producto.Codigo,
                Producto = d.Producto.NombreProducto,
                Categoria = d.Producto.Categoria?.Descripcion ?? string.Empty,
                Precio = d.Producto.Precio,
                Cantidad = d.Cantidad,
                StockDisponible = d.Producto.Estado ? d.Producto.Stock : 0,
                Total = d.Producto.Precio * d.Cantidad,
                StockSuficiente = d.Producto.Estado && d.Cantidad <= d.Producto.Stock
            })
            .ToList();

        var precio = _descuento.Calcular(detalles.Sum(d => d.Total));

        return new CarritoDto
        {
            Detalles = detalles,
            CantidadTotal = detalles.Sum(d => d.Cantidad),
            Subtotal = precio.Subtotal,
            PorcDescuento = precio.PorcDescuento,
            Descuento = precio.Descuento,
            Total = precio.Total,
            TieneProblemasStock = detalles.Any(d => !d.StockSuficiente)
        };
    }
}
