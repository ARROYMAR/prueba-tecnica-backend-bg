using CarritoApi.Data;
using CarritoApi.DTOs.Ordenes;
using CarritoApi.Entities;
using CarritoApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CarritoApi.Services;

public class OrdenService : IOrdenService
{
    private readonly AppDbContext _db;
    private readonly IDescuentoService _descuento;
    private readonly ILogger<OrdenService> _logger;

    public OrdenService(AppDbContext db, IDescuentoService descuento, ILogger<OrdenService> logger)
    {
        _db = db;
        _descuento = descuento;
        _logger = logger;
    }

    public async Task<OrdenDto> FinalizarCompraAsync(int usuarioId, CancellationToken ct = default)
    {
        // Todo ocurre en una transacción: si algún producto no tiene stock,
        // se hace rollback y no queda ni la orden ni stock descontado.
        await using var transaccion = await _db.Database.BeginTransactionAsync(ct);

        var carrito = await _db.CarritosCompra
            .Include(c => c.Detalles)
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId, ct);

        if (carrito is null || carrito.Detalles.Count == 0)
            throw new SolicitudInvalidaException("El carrito está vacío.");

        var detallesOrden = new List<OrdenDetalle>();

        foreach (var detalle in carrito.Detalles.OrderBy(d => d.ProductoId)) // orden fijo: evita deadlocks entre compras simultáneas
        {
            // Descuento atómico: el UPDATE solo afecta la fila si todavía hay stock suficiente.
            // Así dos compras simultáneas no pueden dejar el stock en negativo.
            var filasActualizadas = await _db.Productos
                .Where(p => p.Id == detalle.ProductoId && p.Estado && p.Stock >= detalle.Cantidad)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, p => p.Stock - detalle.Cantidad), ct);

            if (filasActualizadas == 0)
            {
                var stockDisponible = await _db.Productos.AsNoTracking()
                    .Where(p => p.Id == detalle.ProductoId)
                    .Select(p => p.Estado ? p.Stock : 0)
                    .FirstOrDefaultAsync(ct);

                _logger.LogWarning(
                    "Compra rechazada para usuario {UsuarioId}: stock insuficiente de {ProductoId} (pedido {Cantidad}, disponible {Stock}).",
                    usuarioId, detalle.ProductoId, detalle.Cantidad, stockDisponible);

                throw new StockInsuficienteException(detalle.Producto.NombreProducto, stockDisponible);
            }

            detallesOrden.Add(new OrdenDetalle
            {
                ProductoId = detalle.ProductoId,
                ProductoCodigo = detalle.Producto.Codigo,
                ProductoNombre = detalle.Producto.NombreProducto,
                Precio = detalle.Producto.Precio,
                Cantidad = detalle.Cantidad,
                Total = detalle.Producto.Precio * detalle.Cantidad
            });
        }

        var precio = _descuento.Calcular(detallesOrden.Sum(d => d.Total));

        var orden = new Orden
        {
            UsuarioId = usuarioId,
            OrdenCreacion = DateTime.UtcNow,
            Subtotal = precio.Subtotal,
            PorcDescuento = precio.PorcDescuento,
            Descuento = precio.Descuento,
            Total = precio.Total,
            Detalles = detallesOrden
        };

        _db.Ordenes.Add(orden);

        // Vaciar el carrito.
        _db.CarritoCompraDetalles.RemoveRange(carrito.Detalles.ToList());
        carrito.FechaModificacion = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        await transaccion.CommitAsync(ct);

        _logger.LogInformation("Orden {OrdenId} creada para usuario {UsuarioId}. Total {Total}.", orden.Id, usuarioId, orden.Total);

        return ADto(orden);
    }

    public async Task<IReadOnlyList<OrdenResumenDto>> ObtenerHistorialAsync(int usuarioId, CancellationToken ct = default)
    {
        return await _db.Ordenes
            .AsNoTracking()
            .Where(o => o.UsuarioId == usuarioId)
            .OrderByDescending(o => o.OrdenCreacion)
            .ThenByDescending(o => o.Id)
            .Select(o => new OrdenResumenDto
            {
                Id = o.Id,
                OrdenCreacion = o.OrdenCreacion,
                CantidadTotal = o.Detalles.Sum(d => d.Cantidad),
                Subtotal = o.Subtotal,
                PorcDescuento = o.PorcDescuento,
                Descuento = o.Descuento,
                Total = o.Total
            })
            .ToListAsync(ct);
    }

    public async Task<OrdenDto> ObtenerPorIdAsync(int usuarioId, int ordenId, CancellationToken ct = default)
    {
        // Se filtra por usuario: nadie puede ver órdenes ajenas (responde 404, no 403, para no revelar que existen).
        var orden = await _db.Ordenes
                        .AsNoTracking()
                        .Include(o => o.Detalles)
                        .FirstOrDefaultAsync(o => o.Id == ordenId && o.UsuarioId == usuarioId, ct)
                    ?? throw new NoEncontradoException($"No existe la orden {ordenId}.");

        return ADto(orden);
    }

    private static OrdenDto ADto(Orden orden) => new()
    {
        Id = orden.Id,
        OrdenCreacion = orden.OrdenCreacion,
        CantidadTotal = orden.Detalles.Sum(d => d.Cantidad),
        Subtotal = orden.Subtotal,
        PorcDescuento = orden.PorcDescuento,
        Descuento = orden.Descuento,
        Total = orden.Total,
        Detalles = orden.Detalles
            .OrderBy(d => d.ProductoNombre)
            .Select(d => new OrdenDetalleDto
            {
                ProductoId = d.ProductoId,
                ProductoCodigo = d.ProductoCodigo,
                ProductoNombre = d.ProductoNombre,
                Precio = d.Precio,
                Cantidad = d.Cantidad,
                Total = d.Total
            })
            .ToList()
    };
}
