namespace Gdac.Auth.Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, Guid sessionId, IEnumerable<string>? scopes = null);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);

    /// <summary>
    /// Extrai o sessionId do access token sem validar expiração.
    /// Retorna null se o token for inválido estruturalmente.
    /// </summary>
    Guid? ExtractSessionId(string accessToken);

    string GetPublicKeyJwks();
}
