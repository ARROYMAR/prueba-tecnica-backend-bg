using System.Linq.Expressions;
using CarritoApi.Data;
using CarritoApi.DTOs.Categorias;
using CarritoApi.DTOs.Comun;
using CarritoApi.DTOs.Productos;
using CarritoApi.Entities;
using CarritoApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CarritoApi.Services;

public class ProductoService : IProductoService
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProductoService> _logger;

    public ProductoService(AppDbContext db, ILogger<ProductoService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ResultadoPaginado<ProductoDto>> BuscarAsync(ProductoFiltro filtro, CancellationToken ct = default)
    {
        var productos = _db.Productos.AsNoTracking().Where(p => p.Estado);

        if (!string.IsNullOrWhiteSpace(filtro.Buscar))
        {
            var texto = filtro.Buscar.Trim();
            productos = productos.Where(p =>
                p.NombreProducto.Contains(texto) ||
                p.Codigo.Contains(texto) ||
                p.Categoria.Descripcion.Contains(texto));
        }

        if (filtro.CategoriaId.HasValue)
            productos = productos.Where(p => p.CategoriaId == filtro.CategoriaId.Value);

        if (filtro.PrecioMin.HasValue)
            productos = productos.Where(p => p.Precio >= filtro.PrecioMin.Value);

        if (filtro.PrecioMax.HasValue)
            productos = productos.Where(p => p.Precio <= filtro.PrecioMax.Value);

        if (filtro.ConStock)
            productos = productos.Where(p => p.Stock > 0);

        var totalRegistros = await productos.CountAsync(ct);

        var items = await productos
            .OrderBy(p => p.NombreProducto)
            .ThenBy(p => p.Id)
            .Skip((filtro.Pagina - 1) * filtro.TamanoPagina)
            .Take(filtro.TamanoPagina)
            .Select(ADto)
            .ToListAsync(ct);

        return new ResultadoPaginado<ProductoDto>
        {
            Items = items,
            Pagina = filtro.Pagina,
            TamanoPagina = filtro.TamanoPagina,
            TotalRegistros = totalRegistros
        };
    }

    public async Task<ProductoDto> ObtenerPorIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Productos
                   .AsNoTracking()
                   .Where(p => p.Id == id && p.Estado)
                   .Select(ADto)
                   .FirstOrDefaultAsync(ct)
               ?? throw new NoEncontradoException($"No existe un producto con id {id}.");
    }

    public async Task<IReadOnlyList<CategoriaDto>> ObtenerCategoriasAsync(CancellationToken ct = default)
    {
        return await _db.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Descripcion)
            .Select(c => new CategoriaDto { Id = c.Id, Descripcion = c.Descripcion })
            .ToListAsync(ct);
    }

    public async Task<ProductoDto> CrearAsync(ProductoRequest request, CancellationToken ct = default)
    {
        var codigo = request.Codigo.Trim().ToUpperInvariant();
        await ValidarCategoriaAsync(request.CategoriaId, ct);
        await ValidarCodigoUnicoAsync(codigo, idExcluido: null, ct);

        var producto = new Producto();
        Asignar(producto, request, codigo);

        _db.Productos.Add(producto);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Producto creado {ProductoId} ({Codigo}).", producto.Id, producto.Codigo);

        return await ObtenerDtoAsync(producto.Id, ct);
    }

    public async Task<ProductoDto> ActualizarAsync(int id, ProductoRequest request, CancellationToken ct = default)
    {
        var producto = await _db.Productos.FirstOrDefaultAsync(p => p.Id == id, ct)
                       ?? throw new NoEncontradoException($"No existe un producto con id {id}.");

        var codigo = request.Codigo.Trim().ToUpperInvariant();
        await ValidarCategoriaAsync(request.CategoriaId, ct);
        await ValidarCodigoUnicoAsync(codigo, idExcluido: id, ct);

        Asignar(producto, request, codigo);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Producto actualizado {ProductoId}.", id);

        return await ObtenerDtoAsync(id, ct);
    }

    /// <summary>Borrado lógico (Estado = 0): las órdenes antiguas siguen referenciando el producto.</summary>
    public async Task DesactivarAsync(int id, CancellationToken ct = default)
    {
        var producto = await _db.Productos.FirstOrDefaultAsync(p => p.Id == id, ct)
                       ?? throw new NoEncontradoException($"No existe un producto con id {id}.");

        producto.Estado = false;
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Producto desactivado {ProductoId}.", id);
    }

    // ---------- auxiliares ----------

    private static readonly Expression<Func<Producto, ProductoDto>> ADto = p => new ProductoDto
    {
        Id = p.Id,
        Codigo = p.Codigo,
        Producto = p.NombreProducto,
        Descripcion = p.Descripcion,
        CategoriaId = p.CategoriaId,
        Categoria = p.Categoria.Descripcion,
        Precio = p.Precio,
        Stock = p.Stock,
        Estado = p.Estado
    };

    private Task<ProductoDto> ObtenerDtoAsync(int id, CancellationToken ct) =>
        _db.Productos.AsNoTracking().Where(p => p.Id == id).Select(ADto).FirstAsync(ct);

    private static void Asignar(Producto producto, ProductoRequest request, string codigo)
    {
        producto.Codigo = codigo;
        producto.NombreProducto = request.Producto.Trim();
        producto.Descripcion = string.IsNullOrWhiteSpace(request.Descripcion) ? null : request.Descripcion.Trim();
        producto.CategoriaId = request.CategoriaId;
        producto.Precio = request.Precio;
        producto.Stock = request.Stock;
        producto.Estado = request.Estado;
    }

    private async Task ValidarCategoriaAsync(int categoriaId, CancellationToken ct)
    {
        if (!await _db.Categorias.AnyAsync(c => c.Id == categoriaId, ct))
            throw new SolicitudInvalidaException($"La categoría {categoriaId} no existe.");
    }

    private async Task ValidarCodigoUnicoAsync(string codigo, int? idExcluido, CancellationToken ct)
    {
        var existe = await _db.Productos.AnyAsync(p => p.Codigo == codigo && p.Id != idExcluido, ct);
        if (existe)
            throw new ConflictoException("Código duplicado", $"Ya existe un producto con el código '{codigo}'.");
    }
}
