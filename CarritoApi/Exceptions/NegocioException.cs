namespace CarritoApi.Exceptions;

/// <summary>
/// Excepciones de negocio. El ExceptionHandlingMiddleware las convierte en
/// respuestas ProblemDetails con el código HTTP correspondiente.
/// </summary>
public abstract class NegocioException : Exception
{
    protected NegocioException(string titulo, string mensaje) : base(mensaje)
    {
        Titulo = titulo;
    }

    public string Titulo { get; }
    public abstract int CodigoHttp { get; }
}

/// <summary>404: el recurso no existe (o no pertenece al usuario).</summary>
public class NoEncontradoException : NegocioException
{
    public NoEncontradoException(string mensaje) : base("Recurso no encontrado", mensaje) { }
    public override int CodigoHttp => StatusCodes.Status404NotFound;
}

/// <summary>409: la operación choca con el estado actual (código duplicado, producto inactivo...).</summary>
public class ConflictoException : NegocioException
{
    public ConflictoException(string titulo, string mensaje) : base(titulo, mensaje) { }
    public override int CodigoHttp => StatusCodes.Status409Conflict;
}

/// <summary>409 por stock insuficiente: el frontend lo identifica por el código STOCK_INSUFICIENTE.</summary>
public class StockInsuficienteException : ConflictoException
{
    public const string Codigo = "STOCK_INSUFICIENTE";

    public StockInsuficienteException(string producto, int stockDisponible)
        : base("Stock insuficiente",
               stockDisponible <= 0
                   ? $"El producto '{producto}' está agotado."
                   : $"Stock insuficiente para '{producto}'. Disponible: {stockDisponible}.")
    {
        Producto = producto;
        StockDisponible = stockDisponible;
    }

    public string Producto { get; }
    public int StockDisponible { get; }
}

/// <summary>400: la solicitud no tiene sentido en el estado actual (ej. comprar con el carrito vacío).</summary>
public class SolicitudInvalidaException : NegocioException
{
    public SolicitudInvalidaException(string mensaje) : base("Solicitud inválida", mensaje) { }
    public override int CodigoHttp => StatusCodes.Status400BadRequest;
}
