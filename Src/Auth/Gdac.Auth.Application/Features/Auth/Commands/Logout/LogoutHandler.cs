using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gdac.Auth.Application.Features.Auth.Commands.Logout;

public class LogoutHandler(
    ISessionRepository sessions,
    IUnitOfWork unitOfWork,
    ILogger<LogoutHandler> logger
) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand cmd, CancellationToken ct)
    {
        var session = await sessions.FindByIdAsync(cmd.SessionId, ct);
        if (session is null || session.IsRevoked)
            return;

        session.Revoke();
        sessions.Update(session);
        await unitOfWork.CommitAsync(ct);

        logger.LogInformation("Logout realizado. SessionId={SessionId} UserId={UserId}",
            session.Id, session.UserId);
    }
}
