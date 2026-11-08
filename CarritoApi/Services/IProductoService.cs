using CarritoApi.DTOs.Categorias;
using CarritoApi.DTOs.Comun;
using CarritoApi.DTOs.Productos;

namespace CarritoApi.Services;

public interface IProductoService
{
    Task<ResultadoPaginado<ProductoDto>> BuscarAsync(ProductoFiltro filtro, CancellationToken ct = default);
    Task<ProductoDto> ObtenerPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<CategoriaDto>> ObtenerCategoriasAsync(CancellationToken ct = default);

    // Administración (rol Admin)
    Task<ProductoDto> CrearAsync(ProductoRequest request, CancellationToken ct = default);
    Task<ProductoDto> ActualizarAsync(int id, ProductoRequest request, CancellationToken ct = default);
    Task DesactivarAsync(int id, CancellationToken ct = default);
}
