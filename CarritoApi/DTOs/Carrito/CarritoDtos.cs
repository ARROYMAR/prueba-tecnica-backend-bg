using System.ComponentModel.DataAnnotations;

namespace CarritoApi.DTOs.Carrito;

public class CarritoDto
{
    public IReadOnlyList<CarritoDetalleDto> Detalles { get; init; } = Array.Empty<CarritoDetalleDto>();

    /// <summary>Suma de unidades de todos los productos.</summary>
    public int CantidadTotal { get; init; }
    public decimal Subtotal { get; init; }
    public decimal PorcDescuento { get; init; }
    public decimal Descuento { get; init; }
    public decimal Total { get; init; }

    /// <summary>true si algún producto ya no tiene stock suficiente (no se puede comprar).</summary>
    public bool TieneProblemasStock { get; init; }
}

public class CarritoDetalleDto
{
    public int ProductoId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Producto { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public decimal Precio { get; init; }
    public int Cantidad { get; init; }
    public int StockDisponible { get; init; }

    /// <summary>Precio x Cantidad.</summary>
    public decimal Total { get; init; }
    public bool StockSuficiente { get; init; }
}

public class AgregarProductoRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "El producto es obligatorio.")]
    public int ProductoId { get; set; }

    [Range(1, 1000, ErrorMessage = "La cantidad debe estar entre 1 y 1000.")]
    public int Cantidad { get; set; } = 1;
}

public class ActualizarCantidadRequest
{
    [Range(1, 1000, ErrorMessage = "La cantidad debe estar entre 1 y 1000.")]
    public int Cantidad { get; set; }
}
