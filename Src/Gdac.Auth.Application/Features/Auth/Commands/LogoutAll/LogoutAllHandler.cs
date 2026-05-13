using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gdac.Auth.Application.Features.Auth.Commands.LogoutAll;

public class LogoutAllHandler(
    ISessionRepository sessions,
    IUnitOfWork unitOfWork,
    ILogger<LogoutAllHandler> logger
) : IRequestHandler<LogoutAllCommand>
{
    public async Task Handle(LogoutAllCommand cmd, CancellationToken ct)
    {
        await sessions.RevokeAllByUserIdAsync(cmd.UserId, ct);
        await unitOfWork.CommitAsync(ct);

        logger.LogInformation("Logout global realizado. UserId={UserId}", cmd.UserId);
    }
}
