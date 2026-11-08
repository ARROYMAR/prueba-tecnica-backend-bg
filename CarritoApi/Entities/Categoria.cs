namespace CarritoApi.Entities;

/// <summary>Tabla dbo.Categoria.</summary>
public class Categoria
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
