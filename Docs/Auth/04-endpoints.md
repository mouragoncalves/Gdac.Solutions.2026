# Endpoints — Gdac.Auth

Base URL: `/api/v1`

Todos os endpoints retornam `Content-Type: application/json` e seguem o padrão `ProblemDetails` (RFC 7807) para erros.

---

## Auth

### POST /auth/login

Autentica um usuário em uma aplicação.

**Request:**
```json
{
  "email": "usuario@empresa.com",
  "password": "SenhaSegura123!",
  "clientId": "app-erp-gdac",
  "clientSecret": "secret-aqui"
}
```

**Response 200:**
```json
{
  "accessToken": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4...",
  "expiresIn": 900,
  "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "forceChangePassword": false
}
```

**Erros:**
| Status | Mensagem |
|--------|----------|
| 400 | Dados inválidos |
| 401 | `"Usuário ou senha inválidos."` |
| 401 | `"Aplicação não autorizada."` |
| 403 | `"Usuário sem acesso a esta aplicação."` |
| 403 | `"Usuário inativo."` |
| 429 | `"Muitas tentativas. Tente novamente em X segundos."` |

---

### POST /auth/refresh

Renova o access token usando o refresh token.

**Request:**
```json
{
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4..."
}
```

**Headers:** `Authorization: Bearer <access-token>`

**Response 200:** mesmo formato do login

**Erros:**
| Status | Mensagem |
|--------|----------|
| 401 | `"Token de renovação inválido ou expirado."` |
| 401 | `"Sessão encerrada."` |

---

### POST /auth/logout

Encerra a sessão atual.

**Headers:** `Authorization: Bearer <access-token>`

**Response:** `204 No Content`

---

### POST /auth/logout-all

Encerra todas as sessões ativas do usuário em todas as aplicações.

**Headers:** `Authorization: Bearer <access-token>`

**Response:** `204 No Content`

---

### POST /auth/change-password

Troca a senha do usuário autenticado.

**Headers:** `Authorization: Bearer <access-token>`

**Request:**
```json
{
  "currentPassword": "SenhaAtual123!",
  "newPassword": "NovaSenha456@",
  "confirmPassword": "NovaSenha456@"
}
```

**Response:** `204 No Content`

**Erros:**
| Status | Mensagem |
|--------|----------|
| 400 | `"Confirmação de senha não confere."` |
| 400 | `"A nova senha não atende aos requisitos de segurança."` |
| 401 | `"Senha atual incorreta."` |

---

### POST /auth/forgot-password

Solicita redefinição de senha.

**Request:**
```json
{
  "email": "usuario@empresa.com"
}
```

**Response:** `204 No Content` (sempre, para não confirmar existência do e-mail)

---

### POST /auth/reset-password

Redefine a senha usando o token recebido por e-mail.

**Request:**
```json
{
  "token": "token-recebido-por-email",
  "newPassword": "NovaSenha456@",
  "confirmPassword": "NovaSenha456@"
}
```

**Response:** `204 No Content`

**Erros:**
| Status | Mensagem |
|--------|----------|
| 400 | `"Token inválido ou expirado."` |
| 400 | `"A nova senha não atende aos requisitos de segurança."` |

---

### POST /auth/introspect

Valida se uma sessão está ativa. Usado por aplicações clientes.

**Headers:**
```
X-Client-Id: app-erp-gdac
X-Client-Secret: secret-aqui
Authorization: Bearer <access-token-do-usuario>
```

**Response 200:**
```json
{
  "active": true,
  "sub": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "sid": "9b2e4c8a-1234-5678-abcd-ef0123456789",
  "exp": 1715000900
}
```

Sessão inativa:
```json
{
  "active": false
}
```

---

## Session

Todos os endpoints de sessão requerem `Authorization: Bearer <access-token>`.

---

### GET /session/me

Retorna dados básicos do usuário da sessão atual.

**Response 200:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "usuario@empresa.com",
  "mustChangePassword": false,
  "sessionId": "9b2e4c8a-1234-5678-abcd-ef0123456789",
  "sessionCreatedAt": "2026-05-13T10:00:00Z",
  "sessionExpiresAt": "2026-05-13T18:00:00Z"
}
```

---

### GET /session/apps

Retorna as aplicações às quais o usuário tem acesso.

**Response 200:**
```json
{
  "applications": [
    {
      "clientId": "app-erp-gdac",
      "name": "ERP GDAC"
    }
  ]
}
```

---

### GET /session/companies

Retorna as empresas associadas ao usuário.

**Response 200:**
```json
{
  "companies": [
    {
      "id": "guid",
      "externalId": "001",
      "name": "Empresa Exemplo Ltda"
    }
  ]
}
```

---

## Demo

### POST /demo/register

Cria uma conta demo com senha temporária.

**Request:**
```json
{
  "name": "João Silva",
  "email": "joao@empresa.com"
}
```

**Response 200:**
```json
{
  "message": "Conta criada. Verifique seu e-mail para acessar."
}
```

**Erros:**
| Status | Mensagem |
|--------|----------|
| 400 | `"E-mail já cadastrado."` |
| 429 | `"Limite de registros atingido. Tente novamente mais tarde."` |

---

## Admin

Todos os endpoints `/admin/**` requerem:
- `Authorization: Bearer <access-token>`
- Scope `gdac:admin` no token

---

### Usuários

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/admin/users` | Lista usuários (paginado) |
| GET | `/admin/users/{id}` | Detalha usuário |
| POST | `/admin/users` | Cria usuário |
| PUT | `/admin/users/{id}` | Atualiza usuário |
| DELETE | `/admin/users/{id}` | Desativa usuário (soft delete) |
| POST | `/admin/users/{id}/applications` | Associa usuário a aplicação |
| DELETE | `/admin/users/{id}/applications/{appId}` | Remove associação |
| POST | `/admin/users/{id}/companies` | Associa usuário a empresa |
| DELETE | `/admin/users/{id}/companies/{companyId}` | Remove associação |

---

### Aplicações

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/admin/applications` | Lista aplicações |
| GET | `/admin/applications/{id}` | Detalha aplicação |
| POST | `/admin/applications` | Cria aplicação (retorna secret uma única vez) |
| PUT | `/admin/applications/{id}` | Atualiza aplicação |
| DELETE | `/admin/applications/{id}` | Desativa aplicação |
| POST | `/admin/applications/{id}/rotate-secret` | Gera novo client secret |

---

### Empresas

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/admin/companies` | Lista empresas |
| GET | `/admin/companies/{id}` | Detalha empresa |
| POST | `/admin/companies` | Cria empresa |
| PUT | `/admin/companies/{id}` | Atualiza empresa |
| DELETE | `/admin/companies/{id}` | Desativa empresa |

---

## Well-Known

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/.well-known/jwks.json` | Chave pública RSA para verificação de tokens |

---

## Padrão de Erros (ProblemDetails)

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Requisição inválida.",
  "status": 400,
  "detail": "O campo 'email' é obrigatório.",
  "instance": "/api/v1/auth/login",
  "traceId": "00-abc123-def456-00"
}
```

Erros de validação (400):
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Dados inválidos.",
  "status": 400,
  "errors": {
    "email": ["O campo 'email' é obrigatório.", "Formato de e-mail inválido."],
    "password": ["A senha deve ter no mínimo 8 caracteres."]
  },
  "traceId": "00-abc123-def456-00"
}
```

---

## Versionamento

Todos os endpoints seguem o prefixo `/api/v1`. Versões futuras introduzirão `/api/v2` sem remover a anterior durante o período de transição.
