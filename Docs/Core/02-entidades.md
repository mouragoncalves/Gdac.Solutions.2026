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
    public Guid          Id              { get; }
    public string        Name            { get; }
    public string?       TradeName       { get; }
    public string?       Cnpj            { get; }
    public string?       CnpjBase        { get; }   // 8 dígitos (raiz do CNPJ)
    public CompanyType   Type            { get; }   // Client=1, Partner=2, Internal=3
    public CompanyStatus Status          { get; }   // Active=1, Inactive=2, Prospect=3
    public int?          NatureId        { get; }
    public string?       NatureText      { get; }
    public int?          SizeId          { get; }
    public string?       SizeAcronym     { get; }
    public string?       SizeText        { get; }
    public decimal?      Equity          { get; }   // Capital social
    public string?       Jurisdiction    { get; }
    public bool?         SimplesOptant   { get; }
    public DateOnly?     SimplesSince    { get; }
    public bool?         SimeiOptant     { get; }
    public DateOnly?     SimeiSince      { get; }
    public string?       Email           { get; }
    public string?       Phone           { get; }
    public DateTime      CreatedAt       { get; }
    public DateTime      UpdatedAt       { get; }

    public ICollection<CompanyMember>    Members   { get; }
    public ICollection<CompanyOffice>    Offices   { get; }
    public ICollection<UserCompanyLink>  UserLinks { get; }
}
```

**Tabela:** `core_companies`

## CompanyMember

Sócio ou administrador vinculado a uma empresa.

```csharp
public class CompanyMember
{
    public Guid        Id               { get; }
    public Guid        CompanyId        { get; }   // FK → core_companies
    public DateOnly?   Since            { get; }
    public PersonType  PersonType       { get; }   // Natural=1, Legal=2
    public string?     PersonExternalId { get; }
    public string      PersonName       { get; }
    public string?     PersonTaxId      { get; }   // CPF ou CNPJ
    public int?        PersonAge        { get; }
    public int?        RoleId           { get; }
    public string?     RoleText         { get; }
    public string?     AgentName        { get; }
    public string?     AgentTaxId       { get; }
    public int?        AgentRoleId      { get; }
    public string?     AgentRoleText    { get; }
    public DateTime    CreatedAt        { get; }
}
```

**Tabela:** `core_company_members`

## CompanyOffice

Estabelecimento (matriz ou filial) vinculado a uma empresa.

```csharp
public class CompanyOffice
{
    public Guid      Id               { get; }
    public Guid      CompanyId        { get; }   // FK → core_companies
    public string    TaxId            { get; }   // CNPJ 14 dígitos
    public string?   Alias            { get; }
    public DateOnly? Founded          { get; }
    public bool      IsHead           { get; }   // true = matriz
    public int?      StatusId         { get; }
    public string?   StatusText       { get; }
    public DateOnly? StatusDate       { get; }
    public int?      ReasonId         { get; }
    public string?   ReasonText       { get; }
    public int?      MainActivityId   { get; }
    public string?   MainActivityText { get; }
    public DateTime  CreatedAt        { get; }
}
```

**Tabela:** `core_company_offices`

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
public enum PersonType    { Natural = 1, Legal = 2 }
```

## Exceções de domínio

| Exceção | HTTP | Quando usar |
|---------|------|-------------|
| `NotFoundException` | 404 | Entidade não encontrada |
| `DomainException` | 400 | Violação de regra de negócio |
| `UnauthorizedException` | 401 | Acesso negado |
