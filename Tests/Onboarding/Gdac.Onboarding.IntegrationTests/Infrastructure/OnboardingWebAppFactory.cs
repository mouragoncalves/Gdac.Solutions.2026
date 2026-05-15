using Gdac.Onboarding.Domain.Interfaces.Services;
using Gdac.Onboarding.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using System.Security.Cryptography;
using Testcontainers.MySql;

namespace Gdac.Onboarding.IntegrationTests.Infrastructure;

public class OnboardingWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly RSA _testRsa = RSA.Create(2048);

    private readonly MySqlContainer _db = new MySqlBuilder()
        .WithImage("mariadb:11.4")
        .WithDatabase("gdac_onboarding_test")
        .WithUsername("gdac")
        .WithPassword("ci_password")
        .Build();

    public ICoreApiClient CoreApi { get; } = Substitute.For<ICoreApiClient>();
    public IAuthApiClient AuthApi { get; } = Substitute.For<IAuthApiClient>();

    private string ConnectionString =>
        Environment.GetEnvironmentVariable("ConnectionStrings__Default")
        ?? _db.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(cfg => cfg.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:PublicKey"] = _testRsa.ExportSubjectPublicKeyInfoPem(),
            ["Jwt:Issuer"] = "test",
            ["Jwt:Audience"] = "test"
        }));

        builder.ConfigureServices(services =>
        {
            // Replace DbContext with test DB
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<OnboardingDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            services.AddDbContext<OnboardingDbContext>(opts =>
                opts.UseMySql(ConnectionString,
                    new MariaDbServerVersion(new Version(11, 4, 0))));

            // Replace external API clients with mocks
            services.AddScoped(_ => CoreApi);
            services.AddScoped(_ => AuthApi);

            // Accept any JWT in tests
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = false,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    SignatureValidator = (token, _) =>
                        new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(token)
                };
            });
        });
    }

    public async Task InitializeAsync()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ConnectionStrings__Default")))
            await _db.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OnboardingDbContext>();
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ConnectionStrings__Default")))
            await _db.DisposeAsync();
    }
}
