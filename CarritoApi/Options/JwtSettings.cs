namespace CarritoApi.Options;

/// <summary>
/// Se enlaza con la sección "Jwt" de appsettings / variables de entorno.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;

    /// <summary>Clave secreta de firma (mínimo 32 caracteres). Nunca va al repositorio.</summary>
    public string Key { get; set; } = string.Empty;

    public int ExpirationMinutes { get; set; } = 60;
}
