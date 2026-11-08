using CarritoApi.Entities;

namespace CarritoApi.Services;

public interface ITokenService
{
    (string Token, DateTime Expira) CrearToken(Usuario usuario);
}
