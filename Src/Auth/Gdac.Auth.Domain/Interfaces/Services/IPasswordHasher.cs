using Gdac.Auth.Domain.Enums;

namespace Gdac.Auth.Domain.Interfaces.Services;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash, PasswordAlgorithm algorithm);

    /// <summary>
    /// Verifica se o hash foi gerado com algoritmo legado e precisa ser atualizado.
    /// </summary>
    bool NeedsUpgrade(PasswordAlgorithm algorithm);
}
