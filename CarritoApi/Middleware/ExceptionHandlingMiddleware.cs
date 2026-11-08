using CarritoApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CarritoApi.Middleware;

/// <summary>
/// Manejo centralizado de errores:
/// - NegocioException       -> 400 / 404 / 409 con ProblemDetails y mensaje para el usuario.
/// - Cualquier otra excepción -> se registra en el log y se responde 500 sin exponer detalles internos.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NegocioException ex)
        {
            _logger.LogInformation("{Method} {Path} -> {Status}: {Message}",
                context.Request.Method, context.Request.Path, ex.CodigoHttp, ex.Message);

            var problem = new ProblemDetails
            {
                Status = ex.CodigoHttp,
                Title = ex.Titulo,
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            if (ex is StockInsuficienteException stock)
            {
                problem.Extensions["codigo"] = StockInsuficienteException.Codigo;
                problem.Extensions["stockDisponible"] = stock.StockDisponible;
            }

            await WriteAsync(context, problem);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Acceso no autorizado en {Path}", context.Request.Path);
            await WriteAsync(context, new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "No autorizado",
                Detail = "La sesión no es válida. Inicia sesión nuevamente."
            });
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // El cliente cerró la conexión: no es un error del servidor.
            _logger.LogDebug("Solicitud cancelada por el cliente: {Path}", context.Request.Path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en {Method} {Path}", context.Request.Method, context.Request.Path);

            await WriteAsync(context, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Error interno del servidor",
                Detail = "Ocurrió un error inesperado. Intenta de nuevo más tarde."
            });
        }
    }

    private static async Task WriteAsync(HttpContext context, ProblemDetails problem)
    {
        if (context.Response.HasStarted) return;

        context.Response.Clear();
        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem, (System.Text.Json.JsonSerializerOptions?)null, "application/problem+json");
    }
}
