using Gdac.Auth.Domain.Enums;
using Gdac.Auth.Domain.Interfaces.Services;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Gdac.Auth.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    private const int Iterations = 3;
    private const int MemorySize = 65536;
    private const int DegreeOfParallelism = 4;
    private const int HashLength = 32;
    private const int SaltLength = 16;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltLength);
        var hash = ComputeArgon2id(password, salt);
        // Formato: algorithm$iterations$memory$parallelism$salt$hash
        return $"argon2id${Iterations}${MemorySize}${DegreeOfParallelism}" +
               $"${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string storedHash, PasswordAlgorithm algorithm)
    {
        return algorithm switch
        {
            PasswordAlgorithm.Argon2id => VerifyArgon2id(password, storedHash),
            PasswordAlgorithm.BCrypt   => BCrypt.Net.BCrypt.Verify(password, storedHash),
            _                          => false
        };
    }

    public bool NeedsUpgrade(PasswordAlgorithm algorithm) =>
        algorithm != PasswordAlgorithm.Argon2id;

    private static bool VerifyArgon2id(string password, string storedHash)
    {
        try
        {
            var parts = storedHash.Split('$');
            if (parts.Length != 6 || parts[0] != "argon2id")
                return false;

            var iterations = int.Parse(parts[1]);
            var memory = int.Parse(parts[2]);
            var parallelism = int.Parse(parts[3]);
            var salt = Convert.FromBase64String(parts[4]);
            var expectedHash = Convert.FromBase64String(parts[5]);

            var actualHash = ComputeArgon2id(password, salt, iterations, memory, parallelism, expectedHash.Length);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch
        {
            return false;
        }
    }

    private static byte[] ComputeArgon2id(
        string password, byte[] salt,
        int iterations = Iterations,
        int memory = MemorySize,
        int parallelism = DegreeOfParallelism,
        int hashLength = HashLength)
    {
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            Iterations = iterations,
            MemorySize = memory,
            DegreeOfParallelism = parallelism
        };
        return argon2.GetBytes(hashLength);
    }
}
