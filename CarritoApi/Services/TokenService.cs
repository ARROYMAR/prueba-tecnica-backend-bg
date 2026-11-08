using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarritoApi.Entities;
using CarritoApi.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CarritoApi.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;

    public TokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public (string Token, DateTime Expira) CrearToken(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, usuario.NombreUsuario),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.NombreUsuario),
            new(ClaimTypes.Role, usuario.Rol),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);
        var expira = DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes);

        var jwt = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expira,
            signingCredentials: credenciales);

        return (new JwtSecurityTokenHandler().WriteToken(jwt), expira);
    }
}
