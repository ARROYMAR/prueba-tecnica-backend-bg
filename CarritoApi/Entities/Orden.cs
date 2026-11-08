namespace CarritoApi.Entities;

/// <summary>Tabla dbo.Orden.</summary>
public class Orden
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DateTime OrdenCreacion { get; set; }
    public decimal Subtotal { get; set; }
    public decimal PorcDescuento { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }

    public ICollection<OrdenDetalle> Detalles { get; set; } = new List<OrdenDetalle>();
}
