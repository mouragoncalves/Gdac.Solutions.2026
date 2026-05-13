using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace Gdac.Auth.Infrastructure.Security;

public class RsaKeyProvider
{
    public RSA PrivateKey { get; }
    public RSA PublicKey { get; }

    public RsaKeyProvider(IConfiguration configuration)
    {
        var privateKeyPem = configuration["Jwt:PrivateKey"]
            ?? throw new InvalidOperationException("JWT__PrivateKey não configurada.");

        var publicKeyPem = configuration["Jwt:PublicKey"]
            ?? throw new InvalidOperationException("JWT__PublicKey não configurada.");

        PrivateKey = RSA.Create();
        PrivateKey.ImportFromPem(privateKeyPem.Replace("\\n", "\n"));

        PublicKey = RSA.Create();
        PublicKey.ImportFromPem(publicKeyPem.Replace("\\n", "\n"));
    }
}
