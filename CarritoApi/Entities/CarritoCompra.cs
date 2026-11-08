namespace CarritoApi.Entities;

/// <summary>Tabla dbo.CarritoCompra (cabecera, un carrito por usuario).</summary>
public class CarritoCompra
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaModificacion { get; set; }

    public ICollection<CarritoCompraDetalle> Detalles { get; set; } = new List<CarritoCompraDetalle>();
}
