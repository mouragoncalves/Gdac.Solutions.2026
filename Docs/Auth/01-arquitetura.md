# Arquitetura — Gdac.Auth

## Decisões Técnicas

### Implementação JWT customizada (sem OpenIddict)

Optamos por implementação própria de geração e validação de tokens JWT.

**Justificativa:** O OpenIddict é um servidor OAuth 2.0/OIDC completo e carrega abstrações desnecessárias para este cenário. Como a API tem um contrato de autenticação simples e controlado (login por email/senha + client credentials), a implementação própria oferece mais controle, menos dependência de terceiros e código mais direto.

---

### CQRS com MediatR

Toda a camada Application será organizada em Commands e Queries via MediatR.

**Justificativa:** A API tem operações com naturezas muito distintas — autenticação (escrita + leitura de sessão), consulta de sessão ativa (leitura pura), administração (escrita). CQRS torna cada operação isolada, testável e com pipeline de comportamentos (validação, log, tratamento de erro) sem duplicação.

---

### GitHub Actions para CI/CD

**Justificativa:** O repositório já está no GitHub. GitHub Actions elimina a necessidade de infraestrutura externa de CI/CD e tem integração nativa com o repositório.

---

### E-mail com abstração + SMTP padrão

Interface `IEmailService` com implementação SMTP configurável via `appsettings`. A implementação pode ser trocada por SendGrid ou outro provedor sem alterar a lógica de negócio.

---

### Endpoints de administração

Endpoints `/api/v1/admin/**` protegidos por um scope dedicado `gdac:admin` no JWT. Necessários para criar usuários, aplicações, empresas e gerenciar associações — operações que não devem ser expostas pelo fluxo de autenticação normal.

---

### Remoção do campo `PasswordSalt`

O Argon2id incorpora o salt no próprio hash gerado. Um campo separado `PasswordSalt` seria redundante e aumentaria superfície de exposição. Adicionamos `PasswordAlgorithm` para suportar migração gradual de BCrypt → Argon2id.

---

## Estrutura da Solution

```
Gdac.Solutions.2026/
├── Docs/
│   └── Auth/                          ← esta documentação
├── Src/
│   ├── Gdac.Auth.Api/                 ← entrada HTTP, controllers, middleware
│   ├── Gdac.Auth.Application/         ← casos de uso, CQRS, DTOs
│   ├── Gdac.Auth.Domain/              ← entidades, interfaces, exceções de domínio
│   ├── Gdac.Auth.Infrastructure/      ← EF Core, Redis, JWT, Argon2id, e-mail
│   └── Gdac.Auth.Shared/              ← constantes, extensões, helpers
└── Tests/
    ├── Gdac.Auth.UnitTests/           ← testes de units (sem I/O)
    └── Gdac.Auth.IntegrationTests/    ← testes contra banco e Redis reais
```

---

## Camadas e Responsabilidades

### Gdac.Auth.Domain

Núcleo da aplicação. Sem dependências externas.

```
Gdac.Auth.Domain/
├── Entities/
│   ├── User.cs
│   ├── Application.cs
│   ├── Company.cs
│   ├── UserApplication.cs
│   ├── UserCompany.cs
│   ├── Session.cs
│   └── PasswordResetToken.cs
├── Enums/
│   └── PasswordAlgorithm.cs
├── Interfaces/
│   ├── Repositories/
│   │   ├── IUserRepository.cs
│   │   ├── IApplicationRepository.cs
│   │   ├── ICompanyRepository.cs
│   │   ├── ISessionRepository.cs
│   │   ├── IPasswordResetTokenRepository.cs
│   │   └── IUnitOfWork.cs
│   └── Services/
│       ├── IPasswordHasher.cs
│       ├── ITokenService.cs
│       └── IEmailService.cs
└── Exceptions/
    ├── DomainException.cs
    ├── UnauthorizedException.cs
    └── NotFoundException.cs
```

**Regra:** O Domain não conhece EF Core, ASP.NET, Redis, nem nenhum framework de infraestrutura.

---

### Gdac.Auth.Application

Casos de uso. Depende apenas do Domain.

```
Gdac.Auth.Application/
├── Features/
│   ├── Auth/
│   │   ├── Commands/
│   │   │   ├── Login/
│   │   │   ├── Refresh/
│   │   │   ├── Logout/
│   │   │   ├── LogoutAll/
│   │   │   ├── ChangePassword/
│   │   │   ├── ForgotPassword/
│   │   │   └── ResetPassword/
│   │   └── Queries/
│   │       └── Introspect/
│   ├── Session/
│   │   └── Queries/
│   │       ├── GetMe/
│   │       ├── GetApps/
│   │       └── GetCompanies/
│   ├── Demo/
│   │   └── Commands/
│   │       └── RegisterDemo/
│   └── Admin/
│       ├── Commands/
│       │   ├── CreateUser/
│       │   ├── UpdateUser/
│       │   ├── DeactivateUser/
│       │   ├── CreateApplication/
│       │   ├── UpdateApplication/
│       │   ├── CreateCompany/
│       │   ├── UpdateCompany/
│       │   ├── AssignUserApplication/
│       │   ├── RemoveUserApplication/
│       │   ├── AssignUserCompany/
│       │   └── RemoveUserCompany/
│       └── Queries/
│           ├── GetUsers/
│           ├── GetApplications/
│           └── GetCompanies/
├── Common/
│   └── Behaviors/
│       ├── ValidationBehavior.cs      ← FluentValidation via MediatR pipeline
│       └── LoggingBehavior.cs         ← log estruturado de cada command/query
└── DTOs/
    ├── Auth/
    ├── Session/
    └── Admin/
```

Cada `Command` ou `Query` tem:
- `XxxCommand.cs` / `XxxQuery.cs` — request (record)
- `XxxCommandHandler.cs` / `XxxQueryHandler.cs` — handler
- `XxxValidator.cs` — FluentValidation (quando há entrada)

---

### Gdac.Auth.Infrastructure

Implementações concretas. Depende do Domain e de frameworks externos.

```
Gdac.Auth.Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   ├── Configurations/                ← IEntityTypeConfiguration por entidade
│   ├── Repositories/                  ← implementações dos IRepository
│   └── Migrations/
├── Services/
│   ├── PasswordHasher.cs              ← Argon2id + BCrypt
│   ├── TokenService.cs                ← geração e validação JWT (RS256)
│   ├── EmailService.cs                ← SMTP via MailKit
│   └── CacheService.cs                ← Redis
└── Security/
    └── RsaKeyProvider.cs              ← carrega par de chaves RSA do ambiente
```

---

### Gdac.Auth.Api

Camada de entrada HTTP. Depende da Application e Infrastructure (via DI).

```
Gdac.Auth.Api/
├── Controllers/
│   ├── AuthController.cs
│   ├── SessionController.cs
│   ├── DemoController.cs
│   ├── AdminController.cs
│   └── WellKnownController.cs         ← JWKS endpoint
├── Middleware/
│   ├── CorrelationIdMiddleware.cs
│   ├── SecurityHeadersMiddleware.cs
│   └── ExceptionHandlingMiddleware.cs
├── Extensions/
│   ├── ServiceCollectionExtensions.cs
│   └── ApplicationBuilderExtensions.cs
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── appsettings.Staging.json
└── appsettings.Production.json
```

---

### Gdac.Auth.Shared

Sem dependências de domínio ou infraestrutura. Usável por qualquer camada.

```
Gdac.Auth.Shared/
├── Constants/
│   └── ClaimTypes.cs
├── Extensions/
│   └── StringExtensions.cs
└── Helpers/
    └── DateTimeHelper.cs              ← sempre UTC
```

---

## Dependências entre Projetos

```
Api → Application → Domain
Api → Infrastructure → Domain
Shared ← (todos os projetos)
```

A Infrastructure nunca é referenciada pela Application — o Domain define interfaces, a Infrastructure implementa, e a Api registra via DI.
