using CarritoApi.Data;
using CarritoApi.DTOs.Auth;
using CarritoApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarritoApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext db,
        ITokenService tokenService,
        IPasswordHasher<Usuario> passwordHasher,
        ILogger<AuthService> logger)
    {
        _db = db;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var nombreUsuario = request.Usuario.Trim();

        var usuario = await _db.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Estado, cancellationToken);

        if (usuario is null)
        {
            // Mismo resultado que con contraseña incorrecta: no se revela si el usuario existe.
            _logger.LogWarning("Intento de login fallido: usuario '{Usuario}' no existe o está inactivo.", nombreUsuario);
            return null;
        }

        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, request.Password);
        if (resultado == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Intento de login fallido: contraseña incorrecta para '{Usuario}'.", nombreUsuario);
            return null;
        }

        var (token, expira) = _tokenService.CrearToken(usuario);
        _logger.LogInformation("Login correcto para '{Usuario}'.", nombreUsuario);

        return new LoginResponse
        {
            Token = token,
            Expira = expira,
            Usuario = usuario.NombreUsuario,
            Nombre = usuario.Nombre,
            Rol = usuario.Rol
        };
    }
}
