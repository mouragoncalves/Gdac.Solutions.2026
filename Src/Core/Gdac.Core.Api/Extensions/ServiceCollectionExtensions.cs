using FluentValidation;
using Gdac.Core.Application.Common.Behaviors;
using Gdac.Core.Domain.Interfaces.Repositories;
using Gdac.Core.Domain.Interfaces.Services;
using Gdac.Core.Infrastructure.Persistence;
using Gdac.Core.Infrastructure.Persistence.Repositories;
using Gdac.Core.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Security.Cryptography;

namespace Gdac.Core.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(Application.Features.Companies.Commands.CreateCompany.CreateCompanyHandler).Assembly;

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

        services.AddDbContext<CoreDbContext>(options =>
            options.UseMySql(connectionString, new MariaDbServerVersion(new Version(10, 4, 0))));

        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanyMemberRepository, CompanyMemberRepository>();
        services.AddScoped<ICompanyOfficeRepository, CompanyOfficeRepository>();
        services.AddScoped<IUserCompanyLinkRepository, UserCompanyLinkRepository>();
        services.AddScoped<IBlockRecordRepository, BlockRecordRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var publicKeyPem = configuration["Jwt:PublicKey"]
            ?? throw new InvalidOperationException("Jwt:PublicKey não configurada.");

        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem.Replace("\\n", "\n"));
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gdac.Core API", Version = "v1" });
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
