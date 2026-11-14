using System.Security.Claims;
using System.Text;
using InternalVisibleTo;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using TaskBoard.Application.Common.Interfaces;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Auth;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(JwtSettings settings) => _settings = settings;

    public (string Token, DateTime ExpiresAt) Generate(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            { 
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", user.DisplayName)
            
            }),
            Expires = expiresAt,
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };

    var token = new JsonWebTokenHandler().CreateToken(descriptor);
    return (token, expiresAt);
    }
}