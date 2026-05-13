# Entidades — Gdac.Auth

## Modelo de Dados

### User

Representa um usuário que pode autenticar nas aplicações GDAC.

| Campo | Tipo | Observação |
|-------|------|------------|
| Id | Guid | PK |
| Email | string(255) | Unique, índice |
| PasswordHash | string(500) | Hash Argon2id (inclui salt internamente) |
| PasswordAlgorithm | enum | Argon2id \| BCrypt — permite migração gradual |
| IsActive | bool | Soft disable |
| MustChangePassword | bool | Forçar troca no próximo login |
| FailedLoginAttempts | int | Contador para lockout |
| LockoutUntil | DateTime? | UTC — null = não bloqueado |
| CreatedAt | DateTime | UTC |
| UpdatedAt | DateTime | UTC |

> `PasswordSalt` foi removido. O Argon2id incorpora o salt no próprio hash. Um campo separado seria redundante e aumentaria a superfície de exposição.

---

### Application

Representa uma aplicação cliente autorizada a usar a Gdac.Auth.

| Campo | Tipo | Observação |
|-------|------|------------|
| Id | Guid | PK |
| Name | string(150) | Nome legível |
| ClientId | string(100) | Unique, índice — identificador público |
| ClientSecretHash | string(500) | Hash Argon2id do secret |
| IsActive | bool | Desativar sem deletar |
| CreatedAt | DateTime | UTC |

> O `ClientSecret` em texto puro nunca é armazenado. Gerado uma vez, exibido uma vez, armazenado apenas o hash.

---

### Company

Referência a uma empresa nas aplicações GDAC. Armazena apenas dados mínimos para o contexto de autenticação.

| Campo | Tipo | Observação |
|-------|------|------------|
| Id | Guid | PK |
| ExternalId | string(100) | ID da empresa no sistema de origem |
| Name | string(200) | Nome para exibição |
| IsActive | bool | |

> `ExternalId` é o identificador da empresa no banco da aplicação cliente. A Gdac.Auth não replica os dados completos da empresa.

---

### UserApplication

Relacionamento N:N entre usuário e aplicação. Define quais apps o usuário pode acessar.

| Campo | Tipo | Observação |
|-------|------|------------|
| UserId | Guid | FK → User, PK composta |
| ApplicationId | Guid | FK → Application, PK composta |

---

### UserCompany

Relacionamento N:N entre usuário e empresa.

| Campo | Tipo | Observação |
|-------|------|------------|
| UserId | Guid | FK → User, PK composta |
| CompanyId | Guid | FK → Company, PK composta |

---

### Session

Representa uma sessão ativa de um usuário em uma aplicação.

| Campo | Tipo | Observação |
|-------|------|------------|
| Id | Guid | PK |
| UserId | Guid | FK → User, índice |
| ApplicationId | Guid | FK → Application |
| RefreshTokenHash | string(500) | Hash do refresh token — o token real nunca é armazenado |
| IpAddress | string(45) | IPv4 ou IPv6 |
| DeviceInfo | string(500) | User-Agent resumido |
| CreatedAt | DateTime | UTC |
| LastActivityAt | DateTime | UTC — atualizado a cada refresh |
| AbsoluteExpiration | DateTime | UTC — 8h após criação |
| IsRevoked | bool | Logout ou revogação administrativa |

> `ApplicationId` foi adicionado para saber em qual contexto a sessão foi criada e para logout global filtrado por aplicação.

---

### PasswordResetToken

Tokens de redefinição de senha, usados no fluxo forgot-password.

| Campo | Tipo | Observação |
|-------|------|------------|
| Id | Guid | PK |
| UserId | Guid | FK → User, índice |
| TokenHash | string(500) | Hash do token enviado por e-mail |
| ExpiresAt | DateTime | UTC — 30 minutos após geração |
| UsedAt | DateTime? | UTC — null = ainda não usado |
| IsUsed | bool | |

> Um usuário pode ter no máximo 1 token ativo por vez. Gerar novo token invalida o anterior.

---

## Relacionamentos

```
User ──< UserApplication >── Application
User ──< UserCompany    >── Company
User ──< Session
User ──< PasswordResetToken
Session >── Application
```

---

## Índices Obrigatórios

| Tabela | Campo(s) | Tipo |
|--------|----------|------|
| Users | Email | Unique |
| Applications | ClientId | Unique |
| Sessions | UserId | Index |
| Sessions | RefreshTokenHash | Index |
| Sessions | IsRevoked, AbsoluteExpiration | Index composto (queries de sessões ativas) |
| PasswordResetTokens | UserId, IsUsed | Index composto |

---

## Regras de Banco

- **Charset:** UTF8MB4 em todas as tabelas
- **Datas:** sempre UTC, tipo `datetime(6)` no MariaDB
- **Soft delete:** aplicado em `User`, `Application`, `Company` via `IsActive`
- **Migrations:** versionadas, nunca destrutivas em produção
- **GUIDs:** gerados pela aplicação (não pelo banco) para portabilidade
