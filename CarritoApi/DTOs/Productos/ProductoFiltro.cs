using System.ComponentModel.DataAnnotations;

namespace CarritoApi.DTOs.Productos;

/// <summary>Filtros opcionales de GET /api/products (todos por query string).</summary>
public class ProductoFiltro : IValidatableObject
{
    /// <summary>Texto a buscar en nombre, código o categoría.</summary>
    [StringLength(100)]
    public string? Buscar { get; set; }

    public int? CategoriaId { get; set; }

    [Range(0, 1_000_000)]
    public decimal? PrecioMin { get; set; }

    [Range(0, 1_000_000)]
    public decimal? PrecioMax { get; set; }

    /// <summary>true = solo productos con stock disponible.</summary>
    public bool ConStock { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor o igual a 1.")]
    public int Pagina { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100.")]
    public int TamanoPagina { get; set; } = 12;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PrecioMin.HasValue && PrecioMax.HasValue && PrecioMin > PrecioMax)
        {
            yield return new ValidationResult(
                "El precio mínimo no puede ser mayor que el precio máximo.",
                new[] { nameof(PrecioMin), nameof(PrecioMax) });
        }
    }
}
