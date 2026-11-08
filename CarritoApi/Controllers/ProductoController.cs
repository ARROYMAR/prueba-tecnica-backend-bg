using CarritoApi.DTOs.Comun;
using CarritoApi.DTOs.Productos;
using CarritoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarritoApi.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
[Produces("application/json")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    /// <summary>Lista productos activos con búsqueda (nombre, código o categoría), filtros y paginación.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<ProductoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ResultadoPaginado<ProductoDto>>> Listar(
        [FromQuery] ProductoFiltro filtro, CancellationToken ct)
        => Ok(await _productoService.BuscarAsync(filtro, ct));

    /// <summary>Detalle de un producto.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductoDto>> ObtenerPorId(int id, CancellationToken ct)
        => Ok(await _productoService.ObtenerPorIdAsync(id, ct));

    /// <summary>Crea un producto (solo Admin).</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductoDto>> Crear([FromBody] ProductoRequest request, CancellationToken ct)
    {
        var producto = await _productoService.CrearAsync(request, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
    }

    /// <summary>Edita un producto (solo Admin).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    
    public async Task<ActionResult<ProductoDto>> Actualizar(int id, [FromBody] ProductoRequest request, CancellationToken ct)
        => Ok(await _productoService.ActualizarAsync(id, request, ct));

    /// <summary>Desactiva un producto (borrado lógico, solo Admin).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desactivar(int id, CancellationToken ct)
    {
        await _productoService.DesactivarAsync(id, ct);
        return NoContent();
    }
}
