namespace CarritoApi.Entities;

/// <summary>
/// Refleja la tabla dbo.Users creada en CarritoDB.sql.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Rol { get; set; } = "Cliente";
    public bool Estado { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
