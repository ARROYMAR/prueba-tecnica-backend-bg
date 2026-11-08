namespace CarritoApi.DTOs.Productos;

public class ProductoDto
{
    public int Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Producto { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public int CategoriaId { get; init; }


}
