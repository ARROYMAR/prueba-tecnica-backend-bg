using CarritoApi.DTOs.Auth;

namespace CarritoApi.Services;

public interface IAuthService
{
    /// <summary>
    /// Devuelve la respuesta con el token, o null si las credenciales no son válidas.
    /// </summary>
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
