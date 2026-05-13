using MediatR;
using Microsoft.Extensions.Logging;

namespace Gdac.Auth.Application.Common.Behaviors;

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
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Executando {Request}", requestName);

        var response = await next();

        logger.LogInformation("Concluído {Request}", requestName);

        return response;
    }
}
