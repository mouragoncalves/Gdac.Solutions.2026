# Como usar o Swagger — Gdac.Auth

URL local: `http://localhost:5062/swagger`

---

## 1. Fazer login e obter o token

1. Abra o grupo **Auth** e clique em `POST /api/v1/auth/login`
2. Clique em **Try it out**
3. Cole o body abaixo e clique em **Execute**:

```json
{
  "email": "admin@gdac.com.br",
  "password": "sua-senha",
  "clientId": "app-dev-001",
  "clientSecret": "secret-app-dev"
}
```

4. Copie o valor de `accessToken` da resposta

---

## 2. Autorizar o Swagger com o token

1. Clique no botão **Authorize** (cadeado) no topo da página
2. No campo **Value**, cole:
   ```
   Bearer eyJhbGci...
   ```
   > Inclua o prefixo `Bearer ` antes do token
3. Clique em **Authorize** → **Close**

A partir deste momento todos os endpoints protegidos usarão esse token automaticamente.

---

## 3. Testar os endpoints principais

### Login
`POST /auth/login` — obtém access token + refresh token

### Refresh
`POST /auth/refresh`
- Header `Authorization` preenchido automaticamente
- Body:
```json
{ "refreshToken": "token-recebido-no-login" }
```

### Introspect
`POST /auth/introspect`
- Header `Authorization` preenchido automaticamente
- Adicione manualmente os headers:
  - `X-Client-Id: app-dev-001`
  - `X-Client-Secret: secret-app-dev`

### Change Password
`POST /auth/change-password`
```json
{
  "currentPassword": "SenhaAtual",
  "newPassword": "NovaSenha@123",
  "confirmPassword": "NovaSenha@123"
}
```

### Forgot Password
`POST /auth/forgot-password`
```json
{ "email": "usuario@empresa.com" }
```
> Sempre retorna 204 — verifique o Mailpit em `http://localhost:8025` para ver o e-mail

### Reset Password
`POST /auth/reset-password`
```json
{
  "token": "token-recebido-por-email",
  "newPassword": "NovaSenha@123",
  "confirmPassword": "NovaSenha@123"
}
```

### Logout
`POST /auth/logout` — encerra a sessão atual (204)

### Logout All
`POST /auth/logout-all` — encerra todas as sessões do usuário (204)

### Demo Register
`POST /demo/register`
```json
{
  "name": "João Silva",
  "email": "joao@empresa.com",
  "clientId": "app-dev-001",
  "clientSecret": "secret-app-dev"
}
```

---

## 4. Verificar saúde da aplicação

Fora do Swagger, acesse diretamente no browser:

| URL | O que verifica |
|-----|---------------|
| `http://localhost:5062/health/live` | API no ar |
| `http://localhost:5062/health/ready` | API + banco + Redis |
| `http://localhost:5062/.well-known/jwks.json` | Chave pública RSA |

---

## 5. Visualizar e-mails (Mailpit)

Acesse `http://localhost:8025` para ver os e-mails enviados em desenvolvimento (forgot password, demo register).

---

## Observações

- O token expira em **15 minutos** — se um endpoint retornar `401`, repita o passo 1 e 2
- Endpoints marcados com o cadeado requerem autorização
- `POST /auth/introspect` exige os headers `X-Client-Id` e `X-Client-Secret` além do Bearer token — adicione-os manualmente em **Parameters** antes de executar
