using CarritoApi.DTOs.Auth;
using CarritoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarritoApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Autentica al usuario y devuelve un token JWT.</summary>
    /// <response code="200">Credenciales válidas, se devuelve el token.</response>
    /// <response code="400">Datos de entrada inválidos.</response>
    /// <response code="401">Usuario o contraseña incorrectos.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);

        if (response is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Credenciales inválidas",
                Detail = "Usuario o contraseña incorrectos."
            });
        }

        return Ok(response);
    }
}
