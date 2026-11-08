namespace CarritoApi.DTOs.Ordenes;

/// <summary>Fila del historial de compras.</summary>
public class OrdenResumenDto
{
    public int Id { get; init; }
    public DateTime OrdenCreacion { get; init; }

    /// <summary>Suma de unidades compradas.</summary>
    public int CantidadTotal { get; init; }
    public decimal Subtotal { get; init; }
    public decimal PorcDescuento { get; init; }
    public decimal Descuento { get; init; }
    public decimal Total { get; init; }
}

/// <summary>Orden completa con el detalle de productos.</summary>
public class OrdenDto : OrdenResumenDto
{
    public IReadOnlyList<OrdenDetalleDto> Detalles { get; init; } = Array.Empty<OrdenDetalleDto>();
}

public class OrdenDetalleDto
{
    public int ProductoId { get; init; }
    public string ProductoCodigo { get; init; } = string.Empty;
    public string ProductoNombre { get; init; } = string.Empty;
    public decimal Precio { get; init; }
    public int Cantidad { get; init; }
    public decimal Total { get; init; }
}
