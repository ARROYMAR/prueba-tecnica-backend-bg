using System.ComponentModel.DataAnnotations;

namespace CarritoApi.DTOs.Productos;

/// <summary>Datos para crear o editar un producto (solo Admin).</summary>
public class ProductoRequest
{
    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(30, ErrorMessage = "El código admite máximo 30 caracteres.")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre admite máximo 150 caracteres.")]
    public string Producto { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción admite máximo 500 caracteres.")]
    public string? Descripcion { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La categoría es obligatoria.")]
    public int CategoriaId { get; set; }

    [Range(typeof(decimal), "0", "1000000", ErrorMessage = "El precio debe estar entre 0 y 1.000.000.")]
    public decimal Precio { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; }

    public bool Estado { get; set; } = true;
}
