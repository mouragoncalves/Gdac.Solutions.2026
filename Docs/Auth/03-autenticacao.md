# Autenticação — Gdac.Auth

## JWT

### Algoritmo

RS256 (RSA + SHA-256). Par de chaves assimétricas:

- **Chave privada:** usada pela Gdac.Auth para assinar tokens. Nunca exposta.
- **Chave pública:** disponibilizada via `GET /.well-known/jwks.json`. Usada pelas aplicações clientes para verificar assinaturas localmente.

As chaves são carregadas de variáveis de ambiente (`JWT_PRIVATE_KEY`, `JWT_PUBLIC_KEY`) — nunca de arquivos em disco em produção.

---

### Access Token

| Propriedade | Valor |
|-------------|-------|
| Expiração | 15 minutos |
| Algoritmo | RS256 |

**Payload:**

```json
{
  "sub": "user-guid",
  "sid": "session-guid",
  "iss": "https://auth.gdac.com.br",
  "aud": "gdac-apps",
  "iat": 1715000000,
  "exp": 1715000900
}
```

> Não incluir permissões, dados pessoais, empresas, nem regras de negócio no token. As aplicações clientes consultam `/session/me`, `/session/apps` e `/session/companies` para obter contexto.

---

### Refresh Token

| Propriedade | Valor |
|-------------|-------|
| Idle timeout | 1 hora |
| Expiração absoluta | 8 horas |
| Rotação | Obrigatória a cada uso |
| Armazenamento | Apenas hash SHA-256 |

A cada chamada ao `/auth/refresh`, o refresh token atual é invalidado e um novo par (access + refresh) é emitido.

> **Nota de implementação:** o hash do refresh token usa SHA-256 (não Argon2id), pois tokens são longos e aleatórios — a resistência a brute-force é garantida pela entropia do token em si, não pelo custo do hash.

---

## Fluxos

### 1. Login

```
Cliente → POST /api/v1/auth/login
  Body: { email, password, clientId, clientSecret }

1. Valida clientId + clientSecret (Application ativa)
2. Valida email + password (User ativo, sem lockout)
3. Verifica se usuário tem acesso à Application (UserApplication)
4. Verifica tentativas de login — bloqueia se exceder limite
5. Se MustChangePassword = true → retorna status 200 com flag force_change_password
6. Cria Session no banco
7. Gera access token (JWT RS256)
8. Gera refresh token (string aleatória segura), armazena o hash
9. Retorna tokens + metadados da sessão

Resposta:
{
  "accessToken": "eyJ...",
  "refreshToken": "base64-token",
  "expiresIn": 900,
  "sessionId": "guid"
}
```

---

### 2. Refresh

```
Cliente → POST /api/v1/auth/refresh
  Body: { refreshToken }
  Header: Authorization: Bearer <access-token-expirado-ou-válido>

1. Extrai sessionId do access token (sem validar expiração)
2. Busca Session pelo sessionId
3. Valida refresh token contra o hash armazenado
4. Verifica se a sessão não está revogada
5. Verifica idle timeout (LastActivityAt + 1h)
6. Verifica expiração absoluta (AbsoluteExpiration)
7. Invalida o refresh token atual (rotação)
8. Atualiza LastActivityAt
9. Gera novo par de tokens
10. Retorna novos tokens

Resposta: mesmo formato do login
```

---

### 3. Logout

```
Cliente → POST /api/v1/auth/logout
  Header: Authorization: Bearer <access-token>

1. Extrai sessionId do token
2. Marca Session como IsRevoked = true
3. Remove hash do refresh token

Resposta: 204 No Content
```

---

### 4. Logout Global

Encerra todas as sessões ativas do usuário (em todas as aplicações).

```
Cliente → POST /api/v1/auth/logout-all
  Header: Authorization: Bearer <access-token>

1. Extrai userId do token
2. Revoga todas as Sessions ativas do usuário
3. Invalida todos os refresh tokens

Resposta: 204 No Content
```

---

### 5. Change Password

```
Cliente → POST /api/v1/auth/change-password
  Header: Authorization: Bearer <access-token>
  Body: { currentPassword, newPassword, confirmPassword }

1. Valida token e sessão ativa
2. Valida senha atual
3. Valida nova senha (regras de complexidade)
4. Atualiza PasswordHash com Argon2id
5. Define MustChangePassword = false
6. Revoga todas as outras sessões (exceto a atual) — segurança
7. Registra evento no log

Resposta: 204 No Content
```

---

### 6. Forgot Password

```
Cliente → POST /api/v1/auth/forgot-password
  Body: { email }

1. Busca usuário pelo email
   - Se não encontrar: retorna 204 (nunca confirmar existência de email)
2. Invalida tokens de reset anteriores do usuário
3. Gera token de reset aleatório seguro
4. Armazena hash do token com expiração de 30 minutos
5. Envia e-mail com link de reset

Resposta: 204 No Content (sempre, independente de o e-mail existir)
```

---

### 7. Reset Password

```
Cliente → POST /api/v1/auth/reset-password
  Body: { token, newPassword, confirmPassword }

1. Busca PasswordResetToken pelo hash do token recebido
2. Valida: não usado, não expirado
3. Valida nova senha (regras de complexidade)
4. Atualiza PasswordHash com Argon2id
5. Marca token como IsUsed = true
6. Revoga todas as sessões ativas do usuário
7. Registra evento no log

Resposta: 204 No Content
```

---

### 8. Demo Register

```
Cliente → POST /api/v1/demo/register
  Body: { name, email, clientId, clientSecret }

1. Valida clientId + clientSecret (Application ativa)
2. Valida dados do formulário
3. Verifica se e-mail já existe
4. Cria usuário com senha temporária aleatória (16 caracteres)
5. Define MustChangePassword = true
6. Vincula o usuário à Application informada (UserApplication)
7. Envia e-mail com senha temporária
8. Retorna confirmação (sem expor a senha)

Resposta: { "message": "Conta criada. Verifique seu e-mail para acessar." }
```

> As credenciais da aplicação (`clientId`/`clientSecret`) são obrigatórias para determinar a qual aplicação o usuário demo pertence. Sem esse vínculo o login seria impossível.

---

### 9. Introspect (validação de sessão por apps clientes)

Permite que uma aplicação cliente verifique se uma sessão ainda está ativa, sem precisar implementar lógica de sessão própria.

```
Cliente → POST /api/v1/auth/introspect
  Header: Authorization: Bearer <access-token>
  Header: X-Client-Id: <clientId>
  Header: X-Client-Secret: <clientSecret>

1. Valida clientId + clientSecret
2. Valida assinatura do token
3. Verifica se a sessão (sid) ainda existe e não está revogada
4. Verifica expiração absoluta e idle timeout

Resposta (ativa):
{
  "active": true,
  "sub": "user-guid",
  "sid": "session-guid",
  "exp": 1715000900
}

Resposta (inativa/revogada):
{
  "active": false
}
```

---

## Regras de Sessão

| Condição | Comportamento |
|----------|--------------|
| Usuário ativo, usando o sistema | Sessão renovada automaticamente via refresh |
| Inativo > 1 hora | Refresh token expirado por idle timeout — logout forçado |
| Sessão > 8 horas | Expiração absoluta — logout obrigatório independente de atividade |
| Senha alterada | Todas as outras sessões revogadas |
| Reset de senha | Todas as sessões revogadas |
| Desativação do usuário | Todas as sessões revogadas imediatamente |

---

## JWKS Endpoint

```
GET /.well-known/jwks.json

Resposta:
{
  "keys": [
    {
      "kty": "RSA",
      "use": "sig",
      "kid": "gdac-auth-2026",
      "alg": "RS256",
      "n": "...",
      "e": "AQAB"
    }
  ]
}
```

As aplicações clientes devem cachear esta resposta (TTL sugerido: 1 hora) e revalidar apenas quando encontrarem um `kid` desconhecido.
