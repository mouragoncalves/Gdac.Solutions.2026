using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using Gdac.Auth.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gdac.Auth.Application.Features.Auth.Commands.Refresh;

public class RefreshHandler(
    ITokenService tokenService,
    ISessionRepository sessions,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    ILogger<RefreshHandler> logger
) : IRequestHandler<RefreshCommand, RefreshResult>
{
    private static readonly TimeSpan IdleTimeout = TimeSpan.FromHours(1);

    public async Task<RefreshResult> Handle(RefreshCommand cmd, CancellationToken ct)
    {
        var sessionId = tokenService.ExtractSessionId(cmd.AccessToken)
            ?? throw new UnauthorizedException("Token de renovação inválido ou expirado.");

        var session = await sessions.FindByIdAsync(sessionId, ct)
            ?? throw new UnauthorizedException("Sessão encerrada.");

        if (!session.IsActive(IdleTimeout))
            throw new UnauthorizedException("Token de renovação inválido ou expirado.");

        var tokenHash = tokenService.HashRefreshToken(cmd.RefreshToken);
        if (!passwordHasher.Verify(cmd.RefreshToken, session.RefreshTokenHash, Domain.Enums.PasswordAlgorithm.Argon2id))
            throw new UnauthorizedException("Token de renovação inválido ou expirado.");

        var newRefreshToken = tokenService.GenerateRefreshToken();
        var newRefreshTokenHash = tokenService.HashRefreshToken(newRefreshToken);

        session.RotateRefreshToken(newRefreshTokenHash);
        sessions.Update(session);
        await unitOfWork.CommitAsync(ct);

        var accessToken = tokenService.GenerateAccessToken(session.UserId, session.Id);

        logger.LogInformation("Refresh realizado. SessionId={SessionId} Ip={Ip}", session.Id, cmd.IpAddress);

        return new RefreshResult(accessToken, newRefreshToken, 900, session.Id);
    }
}
