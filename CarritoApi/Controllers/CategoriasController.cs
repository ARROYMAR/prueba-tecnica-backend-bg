using CarritoApi.DTOs.Categorias;
using CarritoApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarritoApi.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
[Produces("application/json")]
public class CategoriasController : ControllerBase
{
    private readonly IProductoService _productoService;

    public CategoriasController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    /// <summary>Categorías para el filtro de productos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoriaDto>>> Listar(CancellationToken ct)
        => Ok(await _productoService.ObtenerCategoriasAsync(ct));
}
