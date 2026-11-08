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
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext db,
        ITokenService tokenService,
        IPasswordHasher<User> passwordHasher,
        ILogger<AuthService> logger)
    {
        _db = db;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();

        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive, cancellationToken);

        if (user is null)
        {
            // Mismo resultado que con contraseña incorrecta: no revelamos si el usuario existe.
            _logger.LogWarning("Intento de login fallido: usuario '{Username}' no existe o está inactivo.", username);
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Intento de login fallido: contraseña incorrecta para '{Username}'.", username);
            return null;
        }

        var (token, expiresAt) = _tokenService.CreateToken(user);
        _logger.LogInformation("Login correcto para '{Username}'.", username);

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role
        };
    }
}
