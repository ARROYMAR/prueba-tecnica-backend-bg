namespace CarritoApi.Options;

/// <summary>
/// Regla de descuento (sección "Descuento" de appsettings):
/// si el subtotal supera MontoMinimo se aplica Porcentaje.
/// </summary>
public class DescuentoSettings
{
    public const string SectionName = "Descuento";

    public decimal MontoMinimo { get; set; } = 100m;
    public decimal Porcentaje { get; set; } = 10m;
}
