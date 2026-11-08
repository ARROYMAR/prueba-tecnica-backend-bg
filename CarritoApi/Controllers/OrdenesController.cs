using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenesController : ControllerBase
    {using CarritoApi.DTOs.Ordenes;
using CarritoApi.Extensions;
using CarritoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarritoApi.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
[Produces("application/json")]
public class OrdenesController : ControllerBase
{
    private readonly IOrdenService _ordenService;

    public OrdenesController(IOrdenService ordenService)
    {
        _ordenService = ordenService;
    }

    /// <summary>Finaliza la compra: genera la orden con el carrito actual, descuenta stock y vacía el carrito.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrdenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrdenDto>> FinalizarCompra(CancellationToken ct)
    {
        var orden = await _ordenService.FinalizarCompraAsync(User.ObtenerUsuarioId(), ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = orden.Id }, orden);
    }

    /// <summary>Historial de compras del usuario (más recientes primero).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrdenResumenDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrdenResumenDto>>> Historial(CancellationToken ct)
        => Ok(await _ordenService.ObtenerHistorialAsync(User.ObtenerUsuarioId(), ct));

    /// <summary>Detalle de una compra del usuario.</summary>
    [HttpGet("{id:int}")]
  

    }
}
