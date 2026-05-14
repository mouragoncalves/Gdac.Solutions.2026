# Endpoints — Gdac.Core.Api

Todos os endpoints exigem `Authorization: Bearer <token>`.

## Perfis de Usuário — `/users`

### GET /users
Retorna todos os perfis ativos.

**Response 200:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "fullName": "João Silva",
    "email": "joao@example.com",
    "phone": "+55 11 99999-0000",
    "avatarUrl": null,
    "cpf": "123.456.789-00",
    "birthDate": "1990-05-20",
    "isActive": true
  }
]
```

### GET /users/{id}
Retorna um perfil pelo GUID.

**Response 404** se não encontrado.

### POST /users
Cria um perfil de usuário. O `Id` deve ser o mesmo GUID do usuário no Auth.

**Request body:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fullName": "João Silva",
  "email": "joao@example.com",
  "phone": "+55 11 99999-0000",
  "cpf": "123.456.789-00",
  "birthDate": "1990-05-20"
}
```

**Response 201** com `{ "id": "<guid>" }`.

### PUT /users/{id}
Atualiza dados do perfil (não atualiza email — esse pertence ao Auth).

**Request body:**
```json
{
  "fullName": "João da Silva",
  "phone": "+55 11 98888-1111",
  "avatarUrl": "https://cdn.gdac.com.br/avatars/joao.jpg",
  "cpf": "123.456.789-00",
  "birthDate": "1990-05-20"
}
```

**Response 204** em caso de sucesso.

---

## Empresas — `/companies`

### GET /companies
Retorna todas as empresas.

### GET /companies/{id}
Retorna uma empresa pelo GUID.

**Response 404** se não encontrada.

### POST /companies
Cria uma empresa.

**Request body:**
```json
{
  "name": "ACME Corp",
  "type": 1,
  "tradeName": "ACME",
  "cnpj": "12.345.678/0001-90",
  "email": "contato@acme.com",
  "phone": "+55 11 3000-0000"
}
```

Tipos: `1` = Client, `2` = Partner, `3` = Internal.

**Response 201** com `{ "id": "<guid>" }`.

### PUT /companies/{id}
Atualiza dados da empresa.

**Response 204** em caso de sucesso.

### DELETE /companies/{id}
Desativa a empresa (soft delete — altera `Status` para `Inactive`).

**Response 204** em caso de sucesso.

---

## Vínculos Usuário-Empresa — `/companies/{id}/users`

### GET /companies/{id}/users
Retorna os usuários vinculados à empresa.

**Response 200:**
```json
[
  {
    "userId": "3fa85f64-...",
    "fullName": "João Silva",
    "role": "admin",
    "isActive": true,
    "joinedAt": "2026-01-15T10:30:00Z"
  }
]
```

### POST /companies/{id}/users
Vincula um usuário à empresa com um papel.

**Request body:**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "role": "member"
}
```

**Response 204** em caso de sucesso.

### DELETE /companies/{id}/users/{userId}
Remove o vínculo de um usuário com a empresa.

**Response 204** em caso de sucesso.

---

## Health Checks

| Endpoint | Descrição |
|----------|-----------|
| `GET /health/live` | Liveness — retorna 200 se o processo está vivo |
| `GET /health/ready` | Readiness — verifica conectividade com o banco |

**Response /health/ready:**
```json
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy"
  }
}
```

---

## Respostas de erro (RFC 7807 Problem Details)

```json
{
  "title": "Dados inválidos.",
  "status": 400,
  "errors": {
    "Name": ["'Name' deve ser informado."]
  },
  "traceId": "00-abc123-def456-00",
  "instance": "/companies"
}
```

| Status | Quando |
|--------|--------|
| 400 | Validação falhou ou regra de negócio violada |
| 401 | Token ausente, inválido ou expirado |
| 404 | Recurso não encontrado |
| 500 | Erro interno (detalhes nos logs) |
