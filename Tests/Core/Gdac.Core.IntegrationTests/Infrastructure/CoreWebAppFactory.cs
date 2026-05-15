using Gdac.Core.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Testcontainers.MySql;

namespace Gdac.Core.IntegrationTests.Infrastructure;

public class CoreWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public static readonly RSA TestRsa = RSA.Create(2048);

    private readonly MySqlContainer _db = new MySqlBuilder()
        .WithImage("mariadb:11.4")
        .WithDatabase("gdac_core_test")
        .WithUsername("gdac")
        .WithPassword("ci_password")
        .Build();

    private string ConnectionString =>
        Environment.GetEnvironmentVariable("ConnectionStrings__Default")
        ?? _db.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(cfg => cfg.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:PublicKey"] = TestRsa.ExportSubjectPublicKeyInfoPem(),
            ["Jwt:Issuer"] = "test",
            ["Jwt:Audience"] = "test"
        }));

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CoreDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            services.AddDbContext<CoreDbContext>(opts =>
                opts.UseMySql(ConnectionString,
                    new MariaDbServerVersion(new Version(11, 4, 0))));

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new RsaSecurityKey(TestRsa),
                    ValidateIssuer           = false,
                    ValidateAudience         = false,
                    ValidateLifetime         = true,
                    ClockSkew                = TimeSpan.Zero
                };
            });
        });
    }

    public async Task InitializeAsync()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ConnectionStrings__Default")))
            await _db.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ConnectionStrings__Default")))
            await _db.DisposeAsync();
    }
}
