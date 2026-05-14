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

## Dados CNPJ — `/companies/{id}/cnpj-data`

### PUT /companies/{id}/cnpj-data
Sincroniza campos de dados públicos do CNPJ na empresa (natureza jurídica, porte, Simples Nacional, SIMEI e capital social).

**Request body:**
```json
{
  "cnpjBase": "12345678",
  "natureId": 2062,
  "natureText": "Sociedade Empresária Limitada",
  "sizeId": 5,
  "sizeAcronym": "ME",
  "sizeText": "Micro Empresa",
  "equity": 10000.00,
  "jurisdiction": "SP",
  "simplesOptant": true,
  "simplesSince": "2020-01-01",
  "simeiOptant": false,
  "simeiSince": null
}
```

**Response 204** em caso de sucesso.

---

## Sócios — `/companies/{id}/members`

### GET /companies/{id}/members
Lista todos os sócios da empresa.

**Response 200:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "since": "2020-03-15",
    "personType": 1,
    "personName": "João da Silva",
    "personTaxId": "123.456.789-00",
    "personAge": 45,
    "roleId": 49,
    "roleText": "Sócio-Administrador",
    "agentName": null,
    "agentTaxId": null,
    "agentRoleId": null,
    "agentRoleText": null
  }
]
```

`personType`: `1` = Natural (pessoa física), `2` = Legal (pessoa jurídica).

### PUT /companies/{id}/members
Substitui todos os sócios da empresa (operação full-replace).

**Request body:**
```json
{
  "members": [
    {
      "personType": 1,
      "personName": "João da Silva",
      "roleId": 49,
      "roleText": "Sócio-Administrador",
      "since": "2020-03-15",
      "personExternalId": "ext-123",
      "personTaxId": "123.456.789-00",
      "personAge": 45,
      "agentName": null,
      "agentTaxId": null,
      "agentRoleId": null,
      "agentRoleText": null
    }
  ]
}
```

Campos obrigatórios: `personType`, `personName`, `roleId`, `roleText`.  
Campos opcionais: `since`, `personExternalId`, `personTaxId`, `personAge`, `agentName`, `agentTaxId`, `agentRoleId`, `agentRoleText`.

**Response 204** em caso de sucesso.

---

## Estabelecimentos — `/companies/{id}/offices`

### GET /companies/{id}/offices
Lista todos os estabelecimentos (CNPJs filiais/matriz) da empresa.

**Response 200:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "taxId": "12345678000190",
    "alias": "Filial SP",
    "founded": "2018-06-01",
    "isHead": false,
    "statusId": 2,
    "statusText": "Ativa",
    "statusDate": "2018-06-01",
    "reasonId": null,
    "reasonText": null,
    "mainActivityId": 6201500,
    "mainActivityText": "Desenvolvimento de programas de computador sob encomenda"
  }
]
```

### PUT /companies/{id}/offices
Substitui todos os estabelecimentos da empresa (operação full-replace).

**Request body:**
```json
{
  "offices": [
    {
      "taxId": "12345678000190",
      "isHead": false,
      "statusId": 2,
      "statusText": "Ativa",
      "mainActivityId": 6201500,
      "mainActivityText": "Desenvolvimento de programas de computador sob encomenda",
      "alias": "Filial SP",
      "founded": "2018-06-01",
      "statusDate": "2018-06-01",
      "reasonId": null,
      "reasonText": null
    }
  ]
}
```

Campos obrigatórios: `taxId`, `isHead`, `statusId`, `statusText`, `mainActivityId`, `mainActivityText`.  
Campos opcionais: `alias`, `founded`, `statusDate`, `reasonId`, `reasonText`.

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
