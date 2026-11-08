namespace CarritoApi.Services;

public record ResumenPrecio(decimal Subtotal, decimal PorcDescuento, decimal Descuento, decimal Total);

/// <summary>Única fuente de la regla de descuento: la usan el carrito y la orden.</summary>
public interface IDescuentoService
{
    ResumenPrecio Calcular(decimal subtotal);
}
