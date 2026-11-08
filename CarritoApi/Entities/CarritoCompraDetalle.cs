namespace CarritoApi.Entities;

/// <summary>Tabla dbo.CarritoCompraDetalle.</summary>
public class CarritoCompraDetalle
{
    public int Id { get; set; }
    public int CarritoId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }

    public CarritoCompra CarritoCompra { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}
