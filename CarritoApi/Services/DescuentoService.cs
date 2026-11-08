using CarritoApi.Options;
using Microsoft.Extensions.Options;

namespace CarritoApi.Services;

public class DescuentoService : IDescuentoService
{
    private readonly DescuentoSettings _settings;

    public DescuentoService(IOptions<DescuentoSettings> settings)
    {
        _settings = settings.Value;
    }

    public ResumenPrecio Calcular(decimal subtotal)
    {
        subtotal = Math.Round(subtotal, 2, MidpointRounding.AwayFromZero);

        // "Si el total supera los $100" => estrictamente mayor.
        var porcDescuento = subtotal > _settings.MontoMinimo ? _settings.Porcentaje : 0m;
        var descuento = Math.Round(subtotal * porcDescuento / 100m, 2, MidpointRounding.AwayFromZero);

        return new ResumenPrecio(subtotal, porcDescuento, descuento, subtotal - descuento);
    }
}
