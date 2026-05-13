# Segurança — Gdac.Auth

## Senhas

### Argon2id (padrão)

Configuração recomendada para balancear segurança e performance:

| Parâmetro | Valor | Motivo |
|-----------|-------|--------|
| Iterations | 3 | Custo de tempo |
| MemorySize | 65536 KB (64 MB) | Resistência a GPU |
| DegreeOfParallelism | 4 | Paralelismo |
| HashLength | 32 bytes | Saída segura |

O hash final inclui algoritmo, parâmetros e salt embutidos — portável e auto-descritivo.

### BCrypt (fallback)

Usado apenas para verificar senhas de usuários migrados de sistemas legados. No login bem-sucedido via BCrypt, a senha é re-hashada com Argon2id automaticamente (upgrade transparente).

### Regras de complexidade

Senha deve ter:
- Mínimo 8 caracteres
- Ao menos 1 letra maiúscula
- Ao menos 1 letra minúscula
- Ao menos 1 número
- Ao menos 1 caractere especial

Validado via FluentValidation no backend. Nunca confiar na validação do frontend.

---

## Rate Limit

### Por endpoint

| Endpoint | Limite | Janela | Scope |
|----------|--------|--------|-------|
| POST /auth/login | 5 req | 1 minuto | por IP |
| POST /auth/forgot-password | 3 req | 5 minutos | por IP |
| POST /demo/register | 10 req | 1 hora | por IP |
| POST /auth/refresh | 20 req | 1 minuto | por IP |
| POST /auth/introspect | 60 req | 1 minuto | por ClientId |
| Admin endpoints | 100 req | 1 minuto | por IP |

### Implementação

Usando `System.Threading.RateLimiting` (.NET nativo) com Fixed Window ou Sliding Window, armazenando contadores no Redis para funcionar em múltiplas instâncias.

### Resposta ao exceder limite

```http
HTTP/1.1 429 Too Many Requests
Retry-After: 45
Content-Type: application/problem+json

{
  "title": "Muitas requisições.",
  "status": 429,
  "detail": "Muitas tentativas. Tente novamente em 45 segundos."
}
```

---

## Brute Force e Lockout

Ao falhar o login:

1. Incrementa `FailedLoginAttempts` do usuário
2. Se atingir **5 tentativas:** bloqueia por 15 minutos (`LockoutUntil = Now + 15min`)
3. Se atingir **10 tentativas:** bloqueia por 1 hora
4. Login bem-sucedido zera o contador

O lockout é verificado antes da validação da senha para não revelar se o usuário existe.

---

## Headers de Segurança

Adicionados via middleware em todas as respostas:

```http
X-Frame-Options: DENY
X-Content-Type-Options: nosniff
Referrer-Policy: strict-origin-when-cross-origin
Content-Security-Policy: default-src 'none'
Strict-Transport-Security: max-age=31536000; includeSubDomains; preload
X-Correlation-Id: <guid>
```

`X-Correlation-Id` é gerado por request e propagado nos logs para rastreamento.

---

## CORS

Apenas origens explicitamente cadastradas:

```json
{
  "AllowedOrigins": [
    "https://erp.gdac.com.br",
    "https://app.gdac.com.br",
    "http://localhost:3000",
    "http://localhost:5173"
  ]
}
```

`AllowAnyOrigin` **nunca** será usado.

Os domínios `localhost` são configurados apenas nos ambientes Development e Staging.

---

## Validação de Tokens nas Apps Clientes

Toda aplicação cliente deve validar:

1. **Assinatura:** verificar com a chave pública do JWKS (`/.well-known/jwks.json`)
2. **Expiração:** `exp` não pode estar no passado
3. **Issuer:** deve ser `https://auth.gdac.com.br`
4. **Audience:** deve conter o `clientId` da aplicação
5. **Sessão ativa:** chamar `/auth/introspect` ou confiar no cache local (máximo 1 minuto)

---

## Revogação de Tokens

O access token não é revogável diretamente (JWT stateless). A revogação é feita pela sessão:

- Ao revogar uma sessão (`IsRevoked = true`), o access token ainda é válido por até 15 minutos
- Para cenários críticos (senha comprometida, roubo de conta), usar `/auth/logout-all` que revoga todas as sessões
- Applications que precisam de revogação imediata devem chamar `/auth/introspect` em cada request (não cachear)

---

## Secrets e Configuração

### O que NUNCA vai no código ou repositório

- Chaves privadas RSA
- Client secrets em texto puro
- Connection strings com credenciais
- Chaves de Redis com senha
- Credenciais de SMTP

### Onde ficam

| Ambiente | Mecanismo |
|----------|-----------|
| Development | `dotnet user-secrets` |
| Staging / Production | Variáveis de ambiente via Docker secrets ou orquestrador |

### Variáveis de ambiente obrigatórias

```
JWT__PrivateKey=<RSA private key PEM>
JWT__PublicKey=<RSA public key PEM>
JWT__Issuer=https://auth.gdac.com.br
JWT__Audience=gdac-apps
ConnectionStrings__Default=Server=...;Database=gdac_auth;...
Redis__ConnectionString=redis:6379,password=...
Email__Host=smtp.example.com
Email__Port=587
Email__Username=...
Email__Password=...
```

---

## Logs de Segurança

### O que registrar

| Evento | Dados registrados |
|--------|------------------|
| Login bem-sucedido | userId, applicationId, IP, User-Agent, sessionId |
| Login com falha | email (hash), IP, User-Agent, motivo |
| Logout | userId, sessionId |
| Logout global | userId, quantidade de sessões encerradas |
| Refresh token | sessionId, IP |
| Troca de senha | userId |
| Reset de senha | userId |
| Lockout ativado | userId (hash), IP |
| Token revogado | sessionId, motivo |

### O que NUNCA registrar

- Senha em qualquer forma
- Token completo (access ou refresh)
- Client secret
- Chave privada RSA
- Conteúdo do body de reset password

---

## Proteção do Endpoint Admin

O scope `gdac:admin` é incluído no JWT apenas para usuários administradores. A verificação é feita via policy no ASP.NET Core:

```
[Authorize(Policy = "AdminOnly")]
```

Onde a policy exige o claim `scope` contendo `gdac:admin`. Administradores são criados diretamente no banco na configuração inicial (seed), não há endpoint público de criação de admin.
