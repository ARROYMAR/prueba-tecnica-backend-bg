namespace CarritoApi.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    /// <summary>Fecha/hora (UTC) en que vence el token.</summary>
    public DateTime Expira { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
