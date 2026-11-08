using CarritoApi.Entities;

namespace CarritoApi.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}
