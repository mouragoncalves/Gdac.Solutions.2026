using FluentValidation;
using Gdac.Onboarding.Application.Common.Behaviors;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using Gdac.Onboarding.Domain.Interfaces.Services;
using Gdac.Onboarding.Infrastructure.Persistence;
using Gdac.Onboarding.Infrastructure.Persistence.Repositories;
using Gdac.Onboarding.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Security.Cryptography;

namespace Gdac.Onboarding.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(Application.Features.Registrations.Commands.SubmitClientRegistration.SubmitClientRegistrationHandler).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default não configurada.");

        services.AddDbContext<OnboardingDbContext>(options =>
            options.UseMySql(connectionString, new MariaDbServerVersion(new Version(10, 4, 0))));

        services.AddScoped<IRegistrationRepository, RegistrationRepository>();
        services.AddScoped<ILeadDistributionConfigRepository, LeadDistributionConfigRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var coreApiBase = configuration["ServiceUrls:CoreApi"] ?? "http://localhost";
        var authApiBase = configuration["ServiceUrls:AuthApi"] ?? "http://localhost";

        services.AddHttpClient("core-api", client =>
        {
            client.BaseAddress = new Uri(coreApiBase);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient("auth-api", client =>
        {
            client.BaseAddress = new Uri(authApiBase);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<ICoreApiClient, CoreApiClient>();
        services.AddScoped<IAuthApiClient, AuthApiClient>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var publicKeyPem = configuration["Jwt:PublicKey"]?.Trim();

        RSA rsa;
        if (string.IsNullOrEmpty(publicKeyPem))
        {
            rsa = RSA.Create(2048);
        }
        else
        {
            rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem.Replace("\\n", "\n"));
        }
        var rsaKey = new RsaSecurityKey(rsa);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = rsaKey,
                    ValidateIssuer           = true,
                    ValidIssuer              = configuration["Jwt:Issuer"],
                    ValidateAudience         = true,
                    ValidAudience            = configuration["Jwt:Audience"],
                    ValidateLifetime         = true,
                    ClockSkew                = TimeSpan.Zero
                };
            });

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddSwaggerIfEnabled(this IServiceCollection services, IWebHostEnvironment env)
    {
        if (!env.IsProduction())
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gdac.Onboarding API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                        []
                    }
                });
            });
        }
        return services;
    }

    public static IServiceCollection AddHealthChecksServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddMySql(configuration.GetConnectionString("Default")!, name: "database");
        return services;
    }
}
