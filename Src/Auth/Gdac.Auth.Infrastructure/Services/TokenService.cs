using Gdac.Auth.Domain.Interfaces.Services;
using Gdac.Auth.Infrastructure.Security;
using Gdac.Auth.Shared.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Gdac.Auth.Infrastructure.Services;

public class TokenService(RsaKeyProvider rsaKeyProvider, IConfiguration configuration) : ITokenService
{
    private const int AccessTokenMinutes = 15;

    public string GenerateAccessToken(Guid userId, Guid sessionId, IEnumerable<string>? scopes = null)
    {
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(GdacClaimTypes.SessionId, sessionId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (scopes?.Any() == true)
            claims.Add(new Claim(GdacClaimTypes.Scope, string.Join(" ", scopes)));

        var key = new RsaSecurityKey(rsaKeyProvider.PrivateKey);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(AccessTokenMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(bytes);
    }

    public Guid? ExtractSessionId(string accessToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();

            // Lê sem validar expiração — necessário para refresh de tokens expirados
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new RsaSecurityKey(rsaKeyProvider.PublicKey),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateLifetime = false
            };

            var principal = handler.ValidateToken(accessToken, validationParameters, out _);
            var sidClaim = principal.FindFirst(GdacClaimTypes.SessionId)?.Value;

            return Guid.TryParse(sidClaim, out var sessionId) ? sessionId : null;
        }
        catch
        {
            return null;
        }
    }

    public string GetPublicKeyJwks()
    {
        var rsa = rsaKeyProvider.PublicKey;
        var parameters = rsa.ExportParameters(false);

        var jwk = new
        {
            keys = new[]
            {
                new
                {
                    kty = "RSA",
                    use = "sig",
                    kid = configuration["Jwt:KeyId"] ?? "gdac-auth-key",
                    alg = "RS256",
                    n = Base64UrlEncoder.Encode(parameters.Modulus!),
                    e = Base64UrlEncoder.Encode(parameters.Exponent!)
                }
            }
        };

        return JsonSerializer.Serialize(jwk);
    }
}
