using CarritoApi.DTOs.Ordenes;

namespace CarritoApi.Services;

public interface IOrdenService
{
    /// <summary>Convierte el carrito del usuario en una orden, descuenta stock y vacía el carrito.</summary>
    Task<OrdenDto> FinalizarCompraAsync(int usuarioId, CancellationToken ct = default);
    Task<IReadOnlyList<OrdenResumenDto>> ObtenerHistorialAsync(int usuarioId, CancellationToken ct = default);
    Task<OrdenDto> ObtenerPorIdAsync(int usuarioId, int ordenId, CancellationToken ct = default);
}
