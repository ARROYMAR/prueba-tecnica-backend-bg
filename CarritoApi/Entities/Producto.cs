namespace CarritoApi.Entities;

/// <summary>Tabla dbo.Producto.</summary>
public class Producto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Columna "Producto". C# no permite que una propiedad se llame igual que su clase,
    /// por eso aquí se llama NombreProducto (el mapeo está en AppDbContext).
    /// </summary>
    public string NombreProducto { get; set; } = string.Empty;

    public string? Descripcion { get; set; }
    public int CategoriaId { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public bool Estado { get; set; } = true;

    public Categoria Categoria { get; set; } = null!;
}
