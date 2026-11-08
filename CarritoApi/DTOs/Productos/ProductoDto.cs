namespace CarritoApi.DTOs.Productos;

public class ProductoDto
{
    public int Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Producto { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public int CategoriaId { get; init; }

    /// <summary>Descripción de la categoría.</summary>
    public string Categoria { get; init; } = string.Empty;
    public decimal Precio { get; init; }
    public int Stock { get; init; }
    public bool Estado { get; init; }
}
