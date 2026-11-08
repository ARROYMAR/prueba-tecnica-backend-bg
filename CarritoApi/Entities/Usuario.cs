namespace CarritoApi.Entities;

/// <summary>Tabla dbo.Usuario.</summary>
public class Usuario
{
    public int Id { get; set; }

    /// <summary>
    /// Columna "Usuario". C# no permite que una propiedad se llame igual que su clase,
    /// por eso aquí se llama NombreUsuario (el mapeo está en AppDbContext).
    /// </summary>
    public string NombreUsuario { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Hash de la contraseña (PasswordHasher de ASP.NET Core Identity), nunca texto plano.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>'Admin' o 'Cliente' (CK_Usuario_Rol).</summary>
    public string Rol { get; set; } = "Cliente";
    public bool Estado { get; set; } = true;
    public DateTime FechaCreacion { get; set; }
}
