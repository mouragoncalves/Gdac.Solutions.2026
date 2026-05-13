using Gdac.Auth.Api.Extensions;
using Gdac.Auth.Api.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using System.Text.Json;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, services, cfg) =>
    {
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .ReadFrom.Services(services)
           .Enrich.FromLogContext()
           .Enrich.WithProperty("Application", "Gdac.Auth");

        var emailCfg = ctx.Configuration.GetSection("Email");
        var host     = emailCfg["Host"];
        var alertTo  = emailCfg["AlertTo"];
        var from     = emailCfg["From"];
        var password = emailCfg["Password"];
        var useSsl   = bool.Parse(emailCfg["UseSsl"] ?? "true");
        var port     = int.Parse(emailCfg["Port"] ?? "587");

        if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(alertTo))
        {
            var socketOptions = useSsl
                ? MailKit.Security.SecureSocketOptions.StartTls
                : MailKit.Security.SecureSocketOptions.None;

            var env = ctx.HostingEnvironment.EnvironmentName;
            var subject = ctx.HostingEnvironment.IsProduction()
                ? $"[URGENTE] [Production] [GDAC Auth] Erro crítico"
                : $"[{env}] [GDAC Auth] Erro crítico";

            cfg.WriteTo.Email(
                from:                     from ?? "noreply@gdac.com.br",
                to:                       alertTo,
                host:                     host,
                port:                     port,
                connectionSecurity:       socketOptions,
                credentials:              new System.Net.NetworkCredential(emailCfg["Username"], password),
                subject:                  subject,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error
            );
        }
    });

    builder.Services.AddControllers();
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddSwaggerIfEnabled(builder.Environment);
    builder.Services.AddHealthChecksServices(builder.Configuration);
    builder.Services.AddResponseCaching();

    builder.Services.AddCors(options =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        options.AddDefaultPolicy(policy =>
            policy.WithOrigins(origins)
                  .AllowAnyHeader()
                  .AllowAnyMethod());
    });

    var app = builder.Build();

    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseMiddleware<SecurityHeadersMiddleware>();

    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseResponseCaching();

    if (!app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gdac.Auth v1"));
    }

    app.MapControllers();

    app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        ResponseWriter = async (ctx, report) =>
        {
            ctx.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.ToDictionary(
                    e => e.Key,
                    e => e.Value.Status.ToString())
            });
            await ctx.Response.WriteAsync(result);
        }
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Falha ao iniciar a aplicação.");
}
finally
{
    Log.CloseAndFlush();
}
