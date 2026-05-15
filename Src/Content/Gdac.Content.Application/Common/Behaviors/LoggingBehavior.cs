using MediatR;
using Microsoft.Extensions.Logging;

namespace Gdac.Content.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var name = typeof(TRequest).Name;
        logger.LogInformation("Executando {Request}", name);
        var response = await next();
        logger.LogInformation("Concluído {Request}", name);
        return response;
    }
}
