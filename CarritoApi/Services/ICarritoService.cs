using CarritoApi.DTOs.Carrito;

namespace CarritoApi.Services;

public interface ICarritoService
{
    Task<CarritoDto> ObtenerAsync(int usuarioId, CancellationToken ct = default);
    Task<CarritoDto> AgregarProductoAsync(int usuarioId, AgregarProductoRequest request, CancellationToken ct = default);
    Task<CarritoDto> ActualizarCantidadAsync(int usuarioId, int productoId, int cantidad, CancellationToken ct = default);
    Task<CarritoDto> EliminarProductoAsync(int usuarioId, int productoId, CancellationToken ct = default);
    Task VaciarAsync(int usuarioId, CancellationToken ct = default);
}
