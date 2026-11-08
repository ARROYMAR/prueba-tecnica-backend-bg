using CarritoApi.DTOs.Carrito;
using CarritoApi.Extensions;
using CarritoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarritoApi.Controllers;

/// <summary>Carrito del usuario autenticado. Todas las operaciones devuelven el carrito recalculado.</summary>
[ApiController]
[Route("api/cart")]
[Authorize]
[Produces("application/json")]
public class CarritoController : ControllerBase
{
    private readonly ICarritoService _carritoService;

    public CarritoController(ICarritoService carritoService)
    {
        _carritoService = carritoService;
    }

    /// <summary>Obtiene el carrito con subtotal, descuento y total.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(CarritoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CarritoDto>> Obtener(CancellationToken ct)
        => Ok(await _carritoService.ObtenerAsync(User.ObtenerUsuarioId(), ct));

    /// <summary>Agrega un producto (o suma cantidad si ya estaba) validando stock.</summary>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CarritoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CarritoDto>> AgregarProducto([FromBody] AgregarProductoRequest request, CancellationToken ct)
        => Ok(await _carritoService.AgregarProductoAsync(User.ObtenerUsuarioId(), request, ct));

    /// <summary>Cambia la cantidad de un producto validando stock.</summary>
    [HttpPut("items/{productoId:int}")]
    [ProducesResponseType(typeof(CarritoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CarritoDto>> ActualizarCantidad(
        int productoId, [FromBody] ActualizarCantidadRequest request, CancellationToken ct)
        => Ok(await _carritoService.ActualizarCantidadAsync(User.ObtenerUsuarioId(), productoId, request.Cantidad, ct));

    /// <summary>Quita un producto del carrito.</summary>
    [HttpDelete("items/{productoId:int}")]
    [ProducesResponseType(typeof(CarritoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CarritoDto>> EliminarProducto(int productoId, CancellationToken ct)
        => Ok(await _carritoService.EliminarProductoAsync(User.ObtenerUsuarioId(), productoId, ct));

    /// <summary>Vacía el carrito.</summary>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Vaciar(CancellationToken ct)
    {
        await _carritoService.VaciarAsync(User.ObtenerUsuarioId(), ct);
        return NoContent();
    }
}
