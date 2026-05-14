# Infraestrutura — Gdac.Core

## Banco de dados

O Core usa o mesmo servidor MariaDB do Auth (`mysql.gdac.com.br`), mas com tabelas prefixadas com `core_` para evitar conflito:

| Tabela | Entidade |
|--------|----------|
| `core_user_profiles` | UserProfile |
| `core_companies` | Company |
| `core_user_company_links` | UserCompanyLink |

### CoreDbContext

```csharp
public class CoreDbContext(DbContextOptions<CoreDbContext> options) : DbContext(options)
{
    public DbSet<UserProfile>     UserProfiles     => Set<UserProfile>();
    public DbSet<Company>         Companies        => Set<Company>();
    public DbSet<UserCompanyLink> UserCompanyLinks => Set<UserCompanyLink>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);
}
```

### Migrations

Geração (design-time):

```bash
dotnet ef migrations add NomeDaMigration \
  --project Src/Core/Gdac.Core.Infrastructure \
  --startup-project Src/Core/Gdac.Core.Api \
  --output-dir Persistence/Migrations
```

Aplicação em produção: automática no startup via `db.Database.Migrate()` em `Program.cs`.

### CoreDbContextFactory

Usada somente em design-time (`dotnet ef`). Aponta para XAMPP local:

```csharp
var connectionString = "Server=/Applications/XAMPP/xamppfiles/var/mysql/mysql.sock;" +
                       "Database=gdac_core_dev;Uid=root;Pwd=;CharSet=utf8mb4;SslMode=None;";
```

## Repositórios

Cada entidade tem seu repositório scoped registrado em `AddInfrastructure`:

| Interface | Implementação |
|-----------|--------------|
| `IUserProfileRepository` | `UserProfileRepository` |
| `ICompanyRepository` | `CompanyRepository` |
| `IUserCompanyLinkRepository` | `UserCompanyLinkRepository` |
| `IUnitOfWork` | `UnitOfWork` |

O `UnitOfWork` chama `_context.SaveChangesAsync()` — os handlers sempre chamam `await _uow.SaveChangesAsync()` após mutações.

## EmailService

Usa MailKit para envio via SMTP KingHost. Configuração via `IConfiguration`:

```csharp
await _client.ConnectAsync(host, port, socketOptions, cancellationToken);
await _client.AuthenticateAsync(username, password, cancellationToken);
await _client.SendAsync(message, cancellationToken);
```

Usado nos casos em que o domínio precisa enviar notificações (ex.: convite para empresa). O envio de alertas de erro crítico é feito pelo Serilog diretamente, não por este serviço.

## Registro de serviços (`AddInfrastructure`)

```csharp
services.AddDbContext<CoreDbContext>(options =>
    options.UseMySql(connectionString, new MariaDbServerVersion(new Version(10, 4, 0))));

services.AddScoped<IUserProfileRepository, UserProfileRepository>();
services.AddScoped<ICompanyRepository, CompanyRepository>();
services.AddScoped<IUserCompanyLinkRepository, UserCompanyLinkRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IEmailService, EmailService>();
```

## Health Check

```csharp
services.AddHealthChecks()
    .AddMySql(connectionString, name: "database");
```

Verificação em `GET /health/ready`. O MySQL health check faz um `SELECT 1` para confirmar conectividade.
