# Entidades de Domínio — Gdac.Core

## UserProfile

Perfil de usuário. O `Id` é o mesmo GUID do `User` no `Gdac.Auth`.

```csharp
public class UserProfile
{
    public Guid     Id        { get; }   // = Auth.User.Id
    public string   FullName  { get; }
    public string   Email     { get; }
    public string?  Phone     { get; }
    public string?  AvatarUrl { get; }
    public string?  Cpf       { get; }
    public DateOnly? BirthDate { get; }
    public bool     IsActive  { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }

    public ICollection<UserCompanyLink> CompanyLinks { get; }
}
```

**Tabela:** `core_user_profiles`

## Company

Empresa — pode ser cliente, parceiro ou interna.

```csharp
public class Company
{
    public Guid          Id         { get; }
    public string        Name       { get; }
    public string?       TradeName  { get; }
    public string?       Cnpj       { get; }
    public CompanyType   Type       { get; }   // Client=1, Partner=2, Internal=3
    public CompanyStatus Status     { get; }   // Active=1, Inactive=2, Prospect=3
    public string?       Email      { get; }
    public string?       Phone      { get; }
    public DateTime      CreatedAt  { get; }
    public DateTime      UpdatedAt  { get; }

    public ICollection<UserCompanyLink> UserLinks { get; }
}
```

**Tabela:** `core_companies`

## UserCompanyLink

Associação N:N entre usuário e empresa. Armazena o papel do usuário na empresa.

```csharp
public class UserCompanyLink
{
    public Guid     UserId    { get; }   // FK → core_user_profiles
    public Guid     CompanyId { get; }   // FK → core_companies
    public string   Role      { get; }   // ex.: "admin", "member", "viewer"
    public bool     IsActive  { get; }
    public DateTime JoinedAt  { get; }
}
```

**Tabela:** `core_user_company_links` (chave primária composta: UserId + CompanyId)

## Enums

```csharp
public enum CompanyType   { Client = 1, Partner = 2, Internal = 3 }
public enum CompanyStatus { Active = 1, Inactive = 2, Prospect = 3 }
```

## Exceções de domínio

| Exceção | HTTP | Quando usar |
|---------|------|-------------|
| `NotFoundException` | 404 | Entidade não encontrada |
| `DomainException` | 400 | Violação de regra de negócio |
| `UnauthorizedException` | 401 | Acesso negado |
