# Autorização — Gdac.Core

## JWT RS256

O Core **não emite** tokens. Apenas valida tokens emitidos pelo `Gdac.Auth.Api`.

O Auth usa RS256 (RSA + SHA-256):
- **Chave privada** (Auth): assina os tokens
- **Chave pública** (Core): verifica a assinatura

Isso garante que o Core nunca pode forjar um token — só o Auth pode.

## Configuração

```json
// appsettings.json
{
  "Jwt": {
    "Issuer":    "https://auth.gdac.com.br",
    "Audience":  "gdac-apps"
    // PublicKey vem do .env / variável de ambiente
  }
}
```

```csharp
// ServiceCollectionExtensions.cs
var publicKeyPem = configuration["Jwt:PublicKey"]
    ?? throw new InvalidOperationException("Jwt:PublicKey não configurada.");

var rsa = RSA.Create();
rsa.ImportFromPem(publicKeyPem.Replace("\\n", "\n"));
var rsaKey = new RsaSecurityKey(rsa);

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = rsaKey,
            ValidateIssuer           = true,
            ValidIssuer              = configuration["Jwt:Issuer"],
            ValidateAudience         = true,
            ValidAudience            = configuration["Jwt:Audience"],
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.Zero
        };
    });
```

## Usando o token

Todos os endpoints do Core exigem autenticação via `[Authorize]`. O cliente obtém o token no Auth e o envia no header:

```http
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
```

O `sub` claim do token contém o `UserId` (GUID), que identifica o usuário nos perfis do Core.

## Chave pública por ambiente

| Ambiente Core | Chave pública esperada |
|--------------|----------------------|
| Development | Chave pública do Auth Development (gerada localmente) |
| Staging | Chave pública do Auth Staging |
| Production | Chave pública do Auth Production |

A chave é injetada via variável de ambiente `Jwt__PublicKey` no `.env`.
