using FluentValidation;
using Gdac.Onboarding.Domain.Exceptions;
using System.Text.Json;

namespace Gdac.Onboarding.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            await WriteProblemAsync(context, 400, "Dados inválidos.", errors);
        }
        catch (UnauthorizedException ex) { await WriteProblemAsync(context, 401, ex.Message); }
        catch (NotFoundException ex)     { await WriteProblemAsync(context, 404, ex.Message); }
        catch (DomainException ex)       { await WriteProblemAsync(context, 400, ex.Message); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro inesperado em {Path}", context.Request.Path);
            await WriteProblemAsync(context, 500, "Erro interno do servidor.");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int status, string detail,
        Dictionary<string, string[]>? errors = null)
    {
        context.Response.StatusCode  = status;
        context.Response.ContentType = "application/problem+json";
        var traceId = context.Items["X-Correlation-Id"]?.ToString() ?? context.TraceIdentifier;
        object problem = errors is not null
            ? new { title = detail, status, errors, traceId, instance = context.Request.Path.Value }
            : new { title = detail, status, traceId, instance = context.Request.Path.Value };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
