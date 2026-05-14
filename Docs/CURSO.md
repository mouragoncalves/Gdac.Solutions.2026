# GDAC Platform — Guia de Estudo e Curso

Passo a passo completo de tudo que foi construído e está em funcionamento 100% em produção. Serve como referência de estudo e base para um curso de desenvolvimento de APIs .NET com Clean Architecture, CQRS, Docker e CI/CD.

---

## Índice

1. [Visão do produto](#1-visão-do-produto)
2. [Estrutura do repositório (monorepo)](#2-estrutura-do-repositório-monorepo)
3. [Clean Architecture na prática](#3-clean-architecture-na-prática)
4. [Camada de Domínio](#4-camada-de-domínio)
5. [Camada de Application — CQRS com MediatR](#5-camada-de-application--cqrs-com-mediatr)
6. [Camada de Infrastructure — EF Core e Repositórios](#6-camada-de-infrastructure--ef-core-e-repositórios)
7. [Camada de API — Controllers, Middleware e DI](#7-camada-de-api--controllers-middleware-e-di)
8. [Autenticação JWT RS256](#8-autenticação-jwt-rs256)
9. [Observabilidade — Serilog, Logs e Alertas](#9-observabilidade--serilog-logs-e-alertas)
10. [Docker — imagens e Compose](#10-docker--imagens-e-compose)
11. [CI/CD — GitHub Actions](#11-cicd--github-actions)
12. [Infraestrutura VPS e nginx](#12-infraestrutura-vps-e-nginx)
13. [Testes — Unitários e de Integração](#13-testes--unitários-e-de-integração)
14. [Padrões e decisões de projeto](#14-padrões-e-decisões-de-projeto)
15. [Checklist de criação de um novo serviço](#15-checklist-de-criação-de-um-novo-serviço)

---

## 1. Visão do produto

A plataforma GDAC é composta por microsserviços independentes que compartilham o mesmo banco de dados MariaDB e o mesmo sistema de autenticação JWT.

```
Cliente (web/mobile)
        │
        ├─> Gdac.Auth.Api   — registro, login, refresh token, JWT RS256
        └─> Gdac.Core.Api   — perfis de usuário, empresas, vínculos
```

**Princípio fundamental:** o Auth emite tokens, os demais serviços apenas validam. Um usuário faz login no Auth uma vez e usa o mesmo token em todos os serviços.

---

## 2. Estrutura do repositório (monorepo)

```
Gdac.Solutions.2026/
├── Src/
│   ├── Auth/
│   │   ├── Gdac.Auth.Api/
│   │   ├── Gdac.Auth.Application/
│   │   ├── Gdac.Auth.Domain/
│   │   ├── Gdac.Auth.Infrastructure/
│   │   └── Gdac.Auth.Shared/
│   └── Core/
│       ├── Gdac.Core.Api/
│       ├── Gdac.Core.Application/
│       ├── Gdac.Core.Domain/
│       └── Gdac.Core.Infrastructure/
├── Tests/
│   ├── Auth/
│   │   ├── Gdac.Auth.UnitTests/
│   │   └── Gdac.Auth.IntegrationTests/
│   └── Core/
│       ├── Gdac.Core.UnitTests/
│       └── Gdac.Core.IntegrationTests/
├── docker/
│   ├── Auth/
│   │   ├── docker-compose.yml         ← base: build + healthcheck
│   │   ├── docker-compose.prod.yml    ← sobrescreve: image, porta 8080, env prod
│   │   ├── docker-compose.staging.yml ← sobrescreve: image, porta 8081, env stg
│   │   ├── docker-compose.dev.yml     ← sobrescreve: Redis local, Mailpit
│   │   └── nginx/conf.d/              ← configs nginx por ambiente
│   └── Core/
│       └── (mesma estrutura sem Redis)
├── .github/workflows/
│   ├── ci-auth.yml
│   ├── ci-core.yml
│   ├── deploy-auth-production.yml
│   ├── deploy-auth-staging.yml
│   ├── deploy-core-production.yml
│   └── deploy-core-staging.yml
├── Docs/
│   ├── Auth/
│   ├── Core/
│   ├── Deploy/DEPLOY-SETUP.md
│   └── CURSO.md            ← este arquivo
└── Solution.Gdac.slnx
```

**Por que monorepo?**  
Um único repositório simplifica o CI/CD, a gestão de branches e o versionamento coordenado. O Dockerfile de cada serviço faz COPY do contexto raiz do repositório, permitindo reutilizar packages e estrutura compartilhada.

---

## 3. Clean Architecture na prática

### A regra de dependência

```
Domain      ← nunca depende de nada externo
Application ← depende de Domain
Infrastructure ← depende de Domain (implementa interfaces do Domain)
Api         ← depende de Application; wires up Infrastructure via DI
```

### Por que essa separação?

| Benefício | Exemplo prático |
|-----------|----------------|
| Domínio testável sem I/O | Handlers de Application testam regras sem banco |
| Trocar banco sem alterar regras | Mudar de EF Core para Dapper? Só muda Infrastructure |
| Endpoints independentes do banco | Controller não conhece DbContext |
| Validação centralizada | FluentValidation na Application, não nos Controllers |

### Como a DI conecta tudo

Em `Program.cs` → `ServiceCollectionExtensions.cs`:

```csharp
services.AddApplication();     // MediatR + Validators + Behaviors
services.AddInfrastructure();  // DbContext + Repositories + UnitOfWork
services.AddJwtAuthentication();
```

A camada de Application define interfaces (`IUserProfileRepository`). A Infrastructure implementa. A API injeta via construtores.

---

## 4. Camada de Domínio

A camada de domínio contém **somente** C# puro — nenhuma dependência de framework.

### Entidades

Entidades são classes com identidade (GUID). Preferimos construtores privados com factory methods quando necessário, mas neste projeto usamos inicializadores diretos.

```csharp
// Gdac.Core.Domain/Entities/Company.cs
public class Company
{
    public Guid   Id     { get; private set; } = Guid.NewGuid();
    public string Name   { get; private set; } = default!;
    public CompanyType   Type   { get; private set; }
    public CompanyStatus Status { get; private set; } = CompanyStatus.Active;
    // ...
}
```

### Interfaces de repositório

As interfaces ficam no domínio para que a Application possa depender delas sem conhecer o EF Core:

```csharp
public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<UserProfile>> GetAllAsync(CancellationToken ct);
    Task AddAsync(UserProfile profile, CancellationToken ct);
    void Update(UserProfile profile);
}
```

### Exceções de domínio

```csharp
// Handlers lançam estas exceções
throw new NotFoundException($"Empresa {id} não encontrada.");
throw new DomainException("Empresa já está inativa.");
```

O middleware `ExceptionHandlingMiddleware` converte cada tipo para o status HTTP adequado.

---

## 5. Camada de Application — CQRS com MediatR

### O padrão CQRS

**Command** = altera estado → retorna `Guid` (novo ID) ou `Unit` (sem retorno).  
**Query** = lê estado → retorna DTO.

```
Features/
├── Companies/
│   ├── Commands/
│   │   ├── CreateCompany/
│   │   │   ├── CreateCompanyCommand.cs    ← record com os dados
│   │   │   ├── CreateCompanyHandler.cs   ← lógica
│   │   │   └── CreateCompanyValidator.cs ← FluentValidation
│   │   └── ...
│   └── Queries/
│       ├── GetCompany/
│       │   ├── GetCompanyQuery.cs
│       │   └── GetCompanyHandler.cs
│       └── ...
```

### Um Command completo

```csharp
// Command (record imutável)
public record CreateCompanyCommand(
    string Name, CompanyType Type, string? TradeName,
    string? Cnpj, string? Email, string? Phone) : IRequest<Guid>;

// Validator
public class CreateCompanyValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

// Handler
public class CreateCompanyHandler(ICompanyRepository repo, IUnitOfWork uow)
    : IRequestHandler<CreateCompanyCommand, Guid>
{
    public async Task<Guid> Handle(CreateCompanyCommand cmd, CancellationToken ct)
    {
        var company = new Company { Name = cmd.Name, Type = cmd.Type, /* ... */ };
        await repo.AddAsync(company, ct);
        await uow.SaveChangesAsync(ct);
        return company.Id;
    }
}
```

### Pipeline Behaviors

Os behaviors envolvem **todos** os handlers automaticamente:

```
Request → ValidationBehavior → LoggingBehavior → Handler → Response
```

**ValidationBehavior**: antes do handler, valida via FluentValidation. Se inválido, lança `ValidationException` (capturada pelo middleware → 400).

**LoggingBehavior**: loga início e fim (com duração) de cada request via Serilog.

```csharp
// Registro na DI
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

---

## 6. Camada de Infrastructure — EF Core e Repositórios

### DbContext

```csharp
public class CoreDbContext(DbContextOptions<CoreDbContext> options) : DbContext(options)
{
    public DbSet<UserProfile>     UserProfiles     => Set<UserProfile>();
    public DbSet<Company>         Companies        => Set<Company>();
    public DbSet<UserCompanyLink> UserCompanyLinks => Set<UserCompanyLink>();

    protected override void OnModelCreating(ModelBuilder b)
        => b.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);
}
```

### Configurações de entidade (Fluent API)

```csharp
// Configurations/CompanyConfiguration.cs
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> b)
    {
        b.ToTable("core_companies");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Type).HasConversion<int>();
        b.Property(x => x.Status).HasConversion<int>();
        b.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        b.Property(x => x.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)")
         .ValueGeneratedOnAddOrUpdate();
    }
}
```

O prefixo `core_` evita conflito de nomes com tabelas do Auth no mesmo banco.

### Repositório

```csharp
public class CompanyRepository(CoreDbContext context) : ICompanyRepository
{
    public async Task<Company?> GetByIdAsync(Guid id, CancellationToken ct)
        => await context.Companies.FindAsync([id], ct);

    public async Task AddAsync(Company company, CancellationToken ct)
        => await context.Companies.AddAsync(company, ct);

    public void Update(Company company)
        => context.Companies.Update(company);
}
```

### UnitOfWork

```csharp
public class UnitOfWork(CoreDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}
```

O handler chama `await _uow.SaveChangesAsync()` para persistir. Repositórios nunca chamam `SaveChanges` diretamente — a transação é controlada pelo handler.

### Migrations

```bash
# Criar migration
dotnet ef migrations add InitialCreate \
  --project Src/Core/Gdac.Core.Infrastructure \
  --startup-project Src/Core/Gdac.Core.Api \
  --output-dir Persistence/Migrations

# Aplicar localmente
dotnet ef database update \
  --project Src/Core/Gdac.Core.Infrastructure \
  --startup-project Src/Core/Gdac.Core.Api
```

Em produção, `db.Database.Migrate()` no startup aplica automaticamente.

---

## 7. Camada de API — Controllers, Middleware e DI

### Controller

```csharp
[ApiController]
[Route("companies")]
[Authorize]
public class CompanyController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequest req, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateCompanyCommand(
            req.Name, req.Type, req.TradeName, req.Cnpj, req.Email, req.Phone), ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
}
```

O Controller **não contém lógica**. Só converte HTTP → Command/Query → HTTP.

### Middleware

Três middlewares customizados, registrados nesta ordem:

```csharp
app.UseMiddleware<CorrelationIdMiddleware>();     // 1. injeta X-Correlation-Id
app.UseMiddleware<ExceptionHandlingMiddleware>(); // 2. captura exceções
app.UseMiddleware<SecurityHeadersMiddleware>();   // 3. adiciona headers de segurança
```

**CorrelationIdMiddleware**: gera um GUID por requisição e o propaga em `context.Items["X-Correlation-Id"]`. Aparece na resposta e nos logs para rastreamento.

**ExceptionHandlingMiddleware**: captura exceções de domínio e as converte em respostas RFC 7807:

```csharp
catch (ValidationException ex) { await WriteProblemAsync(ctx, 400, "Dados inválidos.", errors); }
catch (NotFoundException ex)   { await WriteProblemAsync(ctx, 404, ex.Message); }
catch (DomainException ex)     { await WriteProblemAsync(ctx, 400, ex.Message); }
catch (Exception ex)           { await WriteProblemAsync(ctx, 500, "Erro interno."); }
```

**SecurityHeadersMiddleware**: adiciona `X-Content-Type-Options`, `X-Frame-Options`, `X-XSS-Protection`, `Referrer-Policy`.

### Program.cs — estrutura geral

```csharp
// Bootstrap logger (funciona antes do DI)
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(...);       // lê config + sink de e-mail
builder.Services.AddApplication();
builder.Services.AddInfrastructure(config);
builder.Services.AddJwtAuthentication(config);
builder.Services.AddSwaggerIfEnabled(env);
builder.Services.AddHealthChecksServices(config);
builder.Services.AddResponseCaching();
builder.Services.AddCors(...);

var app = builder.Build();

// Migrations no startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health/live", ...);
app.MapHealthChecks("/health/ready", ...);

app.Run();
```

---

## 8. Autenticação JWT RS256

### Por que RSA (assimétrico)?

Com RS256, o Auth guarda a chave privada para assinar. Os outros serviços recebem apenas a chave pública. Mesmo que um serviço seja comprometido, ele não pode criar tokens — não tem a chave privada.

### Fluxo completo

```
1. Usuário POST /auth/login { email, password }
2. Auth verifica credenciais no banco
3. Auth assina JWT com RSA 4096 private key → retorna access_token + refresh_token
4. Cliente usa Bearer token para chamar Core API
5. Core valida assinatura com RSA public key (sem chamar o Auth)
6. Core extrai sub (UserId) do token para identificar o usuário
```

### Estrutura do token JWT

```json
{
  "sub": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "joao@example.com",
  "iss": "https://auth.gdac.com.br",
  "aud": "gdac-apps",
  "exp": 1748123456,
  "iat": 1748119856
}
```

### Geração de chaves RSA

```bash
# Gerar par 4096 bits
openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:4096 -out private.pem
openssl rsa -pubout -in private.pem -out public.pem

# Converter para linha única (para .env)
awk 'NF {printf "%s\\n", $0}' private.pem > private_oneline.txt
awk 'NF {printf "%s\\n", $0}' public.pem  > public_oneline.txt
```

### Leitura da chave em C#

O `.env` armazena a chave com `\n` literal. O código converte:

```csharp
var pem = configuration["Jwt:PublicKey"]!.Replace("\\n", "\n");
var rsa = RSA.Create();
rsa.ImportFromPem(pem);
```

---

## 9. Observabilidade — Serilog, Logs e Alertas

### Configuração Serilog

```csharp
builder.Host.UseSerilog((ctx, services, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)  // lê Serilog: do appsettings
       .ReadFrom.Services(services)                // permite sinks via DI
       .Enrich.FromLogContext()
       .Enrich.WithProperty("Application", "Gdac.Core");

    // Sink de e-mail (somente em prod ou stg se configurado)
    if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(alertTo))
    {
        cfg.WriteTo.Email(
            from: from, to: alertTo, host: host, port: port,
            connectionSecurity: SslOnConnect,
            credentials: new NetworkCredential(username, password),
            subject: "[URGENTE] [Production] [GDAC Core] Erro crítico",
            restrictedToMinimumLevel: LogEventLevel.Error);
    }
});
```

### Níveis por ambiente

| Ambiente | Default | EF Core queries |
|----------|---------|----------------|
| Development | Debug | Information |
| Staging | Information | Warning |
| Production | Information | Warning |

### LoggingBehavior

Cada comando/query passa pelo behavior e é logado:

```
[INF] Executing CreateCompanyCommand
[INF] CreateCompanyCommand completed in 45ms
```

Em caso de falha, a exceção é capturada pelo `ExceptionHandlingMiddleware` e logada com `logger.LogError`.

### Rastreamento com X-Correlation-Id

Toda requisição recebe um ID único:
- Gerado pelo `CorrelationIdMiddleware`
- Propagado nos headers de resposta
- Injetado no contexto Serilog → aparece em todos os logs da requisição
- Incluído nos erros RFC 7807 como `traceId`

---

## 10. Docker — imagens e Compose

### Dockerfile multi-stage

```dockerfile
# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restaurar dependências primeiro (aproveita cache de layer)
COPY ["Src/Core/Gdac.Core.Api/Gdac.Core.Api.csproj", "Src/Core/Gdac.Core.Api/"]
COPY ["Src/Core/Gdac.Core.Application/...csproj", "Src/Core/Gdac.Core.Application/"]
COPY ["Src/Core/Gdac.Core.Domain/...csproj", "Src/Core/Gdac.Core.Domain/"]
COPY ["Src/Core/Gdac.Core.Infrastructure/...csproj", "Src/Core/Gdac.Core.Infrastructure/"]
RUN dotnet restore "Src/Core/Gdac.Core.Api/Gdac.Core.Api.csproj"

# Copiar fonte e publicar
COPY . .
RUN dotnet publish "Src/Core/Gdac.Core.Api/Gdac.Core.Api.csproj" \
    -c Release -o /app/publish --no-restore

# Stage 2: runtime (imagem menor, sem SDK)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Usuário não-root para segurança
RUN groupadd --system --gid 1001 gdac && \
    useradd --system --uid 1001 --gid gdac --no-create-home gdac
COPY --from=build /app/publish .
RUN chown -R gdac:gdac /app
USER gdac

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Gdac.Core.Api.dll"]
```

**Por que copiar os `.csproj` antes do source?** O Docker usa cache de layer. Se só o código fonte mudar (não os packages), a layer de `dotnet restore` é reutilizada — build muito mais rápido.

### Docker Compose multi-arquivo

```bash
# Combinar arquivos base + override de ambiente
docker compose \
  -p gdac-core-prod \        # nome explícito para evitar conflito
  --env-file .env \
  -f docker/Core/docker-compose.yml \         # base
  -f docker/Core/docker-compose.prod.yml \    # override prod
  up -d --remove-orphans
```

**Por que `-p` explícito?** Quando o compose file está em subdiretório, o Docker deriva o nome do projeto pelo diretório da primeira `-f`. Sem `-p`, prod e stg teriam o mesmo nome de projeto, causando conflito de redes e volumes.

---

## 11. CI/CD — GitHub Actions

### Visão geral dos workflows

```
ci-auth.yml           → build + test + docker push (branches: main, staging, develop)
ci-core.yml           → build + test + docker push (branches: main, staging, develop)

deploy-auth-production.yml  → triggered por: ci-auth success on main
deploy-auth-staging.yml     → triggered por: ci-auth success on staging
deploy-core-production.yml  → triggered por: ci-core success on main
deploy-core-staging.yml     → triggered por: ci-core success on staging
```

### CI — etapas

```yaml
jobs:
  build-and-test:
    services:
      mariadb:           # banco real para integration tests (não mock!)
        image: mariadb:11.4
    steps:
      - Checkout
      - Setup .NET 10
      - dotnet restore
      - dotnet build --no-restore -c Release
      - Unit Tests
      - Integration Tests   # usa banco MariaDB do service acima
      - Publish Test Results (dorny/test-reporter)

  docker-build:
    needs: build-and-test
    if: main || staging    # só constrói imagem nessas branches
    steps:
      - Login GHCR
      - Set tags: SHA + latest/staging
      - docker/build-push-action
```

### Deploy — etapas

```yaml
steps:
  - Resolve image tag (SHA do commit ou input manual)
  - SSH no VPS (appleboy/ssh-action)
  - git pull (atualiza compose files e nginx configs)
  - docker pull (baixa a nova imagem)
  - docker compose up -d --remove-orphans (sobe sem downtime)
  - Aguarda health check (/health/ready)
```

### Rollback manual

Se o deploy causar problema:

```bash
# No VPS
cd /opt/gdac/core
IMAGE_TAG=<sha-anterior> docker compose -p gdac-core-prod \
  --env-file .env \
  -f docker/Core/docker-compose.yml \
  -f docker/Core/docker-compose.prod.yml \
  up -d --no-build
```

### Deploy manual via GitHub CLI

```bash
gh workflow run deploy-core-production.yml \
  --repo <owner>/Gdac.Solutions.2026 \
  --field image_tag=<sha>
```

---

## 12. Infraestrutura VPS e nginx

### Topologia

```
Internet
    │ :443 (HTTPS)
    ▼
nginx (host, porta 80/443)
    ├─ auth.gdac.com.br     → 127.0.0.1:8080
    ├─ auth-stg.gdac.com.br → 127.0.0.1:8081
    ├─ core-api.gdac.com.br     → 127.0.0.1:8082
    └─ core-api-stg.gdac.com.br → 127.0.0.1:8083
```

Cada serviço ouve em `127.0.0.1:<porta>` (não exposto para a internet diretamente). O nginx faz TLS termination e proxy reverso.

### nginx config por domínio

```nginx
server {
    listen 443 ssl http2;
    server_name core-api.gdac.com.br;

    ssl_certificate     /etc/letsencrypt/live/gdac.com.br/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/gdac.com.br/privkey.pem;

    location / {
        proxy_pass         http://127.0.0.1:8082;
        proxy_set_header   Host $host;
        proxy_set_header   X-Forwarded-Proto $scheme;
        proxy_set_header   X-Real-IP $remote_addr;
    }
}
```

O certificado é SAN (Subject Alternative Name) — um único cert para todos os subdomínios:

```bash
certbot --nginx \
  -d auth.gdac.com.br \
  -d auth-stg.gdac.com.br \
  -d core-api.gdac.com.br \
  -d core-api-stg.gdac.com.br
```

---

## 13. Testes — Unitários e de Integração

### Testes unitários

Testam handlers, validators e lógica de domínio **sem I/O**. Usam NSubstitute para mock de repositórios:

```csharp
public class CreateCompanyHandlerTests
{
    private readonly ICompanyRepository _repo = Substitute.For<ICompanyRepository>();
    private readonly IUnitOfWork _uow         = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_ValidCommand_ReturnsGuid()
    {
        var handler = new CreateCompanyHandler(_repo, _uow);
        var cmd     = new CreateCompanyCommand("ACME", CompanyType.Client, null, null, null, null);

        var id = await handler.Handle(cmd, default);

        id.Should().NotBeEmpty();
        await _repo.Received(1).AddAsync(Arg.Any<Company>(), default);
        await _uow.Received(1).SaveChangesAsync(default);
    }
}
```

### Testes de integração

Testam endpoints HTTP completos com banco real via Testcontainers:

```csharp
public class CompaniesIntegrationTests : IClassFixture<CoreWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CompaniesIntegrationTests(CoreWebApplicationFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Post_Companies_Returns201()
    {
        var body = new { name = "Test Corp", type = 1 };
        var res  = await _client.PostAsJsonAsync("/companies", body);

        res.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

**Testcontainers.MySql**: sobe um container MariaDB real durante os testes — elimina diferenças entre mock e produção.

### No CI

O CI usa um `service` de MariaDB nativo do GitHub Actions em vez de Testcontainers (mais rápido no runner). A connection string é injetada via variável de ambiente no step de Integration Tests.

---

## 14. Padrões e decisões de projeto

### Records para Commands e Queries

```csharp
public record CreateCompanyCommand(string Name, CompanyType Type, ...) : IRequest<Guid>;
```

Records são imutáveis e têm igualdade por valor — perfeito para commands que não devem ser modificados após criação.

### Minimal responses nos Controllers

Nada de `return Ok(new { ... })` com objetos anônimos grandes. O Controller retorna apenas o que faz sentido para o status HTTP:

- `201 Created` → `{ id }` no body + Location header via `CreatedAtAction`
- `204 No Content` → body vazio
- `200 OK` → o DTO retornado pelo handler

### Sem DbContext fora de Infrastructure

Controllers e Handlers nunca recebem `DbContext` diretamente. A regra de dependência garante que Application não pode importar EF Core.

### Migrations automáticas no startup

```csharp
db.Database.Migrate();
```

Simples e eficaz para este porte. Em sistemas com zero-downtime mais rigoroso, as migrations seriam aplicadas fora do container (ex.: job separado).

### Projeto Docker isolado por ambiente com -p

```bash
-p gdac-core-prod   # produção
-p gdac-core-stg    # staging
```

Evita colisão de nomes de rede e containers quando múltiplos ambientes rodam no mesmo host.

### Feature flags de Swagger

```csharp
if (!env.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(...);
}
```

Swagger nunca é exposto em produção — só dev e staging.

---

## 15. Checklist de criação de um novo serviço

Use este checklist ao criar uma API semelhante ao Auth ou Core:

### 1. Domain
- [ ] Entidades (Entities/)
- [ ] Interfaces de repositório (Interfaces/Repositories/)
- [ ] Exceções de domínio (DomainException, NotFoundException, etc.)
- [ ] Enums necessários

### 2. Application
- [ ] Behaviors: ValidationBehavior + LoggingBehavior
- [ ] Para cada Use Case:
  - [ ] Command ou Query (record : IRequest<T>)
  - [ ] Handler (IRequestHandler<Command, T>)
  - [ ] Validator (AbstractValidator<Command>) se for Command

### 3. Infrastructure
- [ ] DbContext com DbSets e `ApplyConfigurationsFromAssembly`
- [ ] IEntityTypeConfiguration para cada entidade (tabela, colunas, FK)
- [ ] Repositórios implementando as interfaces do Domain
- [ ] UnitOfWork
- [ ] DbContextFactory (design-time, para `dotnet ef`)
- [ ] EmailService (se necessário)
- [ ] Registrar no `AddInfrastructure`

### 4. API
- [ ] Middleware: CorrelationId + ExceptionHandling + SecurityHeaders
- [ ] Controllers por agregado
- [ ] ServiceCollectionExtensions (AddApplication, AddInfrastructure, AddJwt, AddSwagger, AddHealthChecks)
- [ ] Program.cs (Serilog, migrate, middlewares, CORS, map controllers, health)
- [ ] appsettings.json / .Development / .Staging / .Production
- [ ] Dockerfile multi-stage
- [ ] launchSettings.json

### 5. Testes
- [ ] UnitTests.csproj com referências a Domain e Application
- [ ] IntegrationTests.csproj com referências a Api e Infrastructure
- [ ] Pelo menos um teste por handler/validator

### 6. Docker e CI/CD
- [ ] docker-compose.yml (base)
- [ ] docker-compose.prod.yml (porta, image, env prod)
- [ ] docker-compose.staging.yml (porta, image, env stg)
- [ ] docker-compose.dev.yml (mailpit, banco local)
- [ ] nginx/conf.d/production.conf e staging.conf
- [ ] ci-<serviço>.yml
- [ ] deploy-<serviço>-production.yml
- [ ] deploy-<serviço>-staging.yml
- [ ] Adicionar projetos ao Solution.Gdac.slnx

### 7. VPS e Secrets
- [ ] Criar diretórios `/opt/gdac/<serviço>/` e `/opt/gdac/<serviço>-stg/`
- [ ] Clonar repositório em cada diretório
- [ ] Criar arquivo `.env` em cada diretório
- [ ] Adicionar nginx configs e recarregar nginx
- [ ] Expandir certificado SSL com novo domínio
- [ ] Adicionar DNS A records
- [ ] Adicionar GitHub secrets (se novos segredos necessários)
- [ ] Primeira execução manual do CI/CD

### 8. Documentação
- [ ] Docs/<Serviço>/README.md
- [ ] Docs/<Serviço>/01-arquitetura.md
- [ ] Docs/<Serviço>/02-entidades.md
- [ ] Docs/<Serviço>/03-autorizacao.md (ou autenticacao se for Auth)
- [ ] Docs/<Serviço>/04-endpoints.md
- [ ] Docs/<Serviço>/05-infraestrutura.md
- [ ] Atualizar Docs/Deploy/DEPLOY-SETUP.md com nova porta e variáveis
- [ ] Atualizar este CURSO.md com exemplos do novo serviço
