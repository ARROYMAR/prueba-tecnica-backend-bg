using System.Security.Claims;

namespace CarritoApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Id del usuario autenticado, tomado del token JWT.</summary>
    public static int ObtenerUsuarioId(this ClaimsPrincipal usuario)
    {
        var valor = usuario.FindFirstValue(ClaimTypes.NameIdentifier) ?? usuario.FindFirstValue("sub");
        return int.TryParse(valor, out var id)
            ? id
            : throw new UnauthorizedAccessException("El token no contiene un identificador de usuario válido.");
    }
}
