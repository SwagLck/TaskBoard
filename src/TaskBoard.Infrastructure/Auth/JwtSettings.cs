using Microsoft.Identity.Client;

namespace TaskBoard.Infrastructure.Auth;

public class JwtSettings
{
    public const string SectionName ="Jwt";

    public string Issuer {get; set;} = string.Empty;
    public string Audience {get; set;} =string.Empty;
    public string Secret {get; set;} = string.Empty;
    public int ExpiryMinutes {get; set;} = 60;
}