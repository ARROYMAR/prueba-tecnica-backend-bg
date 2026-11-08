namespace CarritoApi.Entities;

/// <summary>
/// Tabla dbo.OrdenDetalle. Guarda una copia del código, nombre y precio del producto
/// al momento de la compra, para que el historial no cambie si luego se edita el producto.
/// </summary>
public class OrdenDetalle
{
    public int Id { get; set; }
    public int OrdenId { get; set; }
    public int ProductoId { get; set; }
    public string ProductoCodigo { get; set; } = string.Empty;
    public string ProductoNombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    public decimal Total { get; set; }

    public Orden Orden { get; set; } = null!;
}
