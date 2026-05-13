using Gdac.Auth.Domain.Enums;
using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using Gdac.Auth.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gdac.Auth.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordHandler(
    IUserRepository users,
    ISessionRepository sessions,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    ILogger<ChangePasswordHandler> logger
) : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand cmd, CancellationToken ct)
    {
        var user = await users.FindByIdAsync(cmd.UserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), cmd.UserId);

        if (!passwordHasher.Verify(cmd.CurrentPassword, user.PasswordHash, user.PasswordAlgorithm))
            throw new UnauthorizedException("Senha atual incorreta.");

        var newHash = passwordHasher.Hash(cmd.NewPassword);
        user.UpdatePassword(newHash, PasswordAlgorithm.Argon2id);
        users.Update(user);

        // Revoga todas as sessões exceto a atual
        var activeSessions = await sessions.FindActiveByUserIdAsync(cmd.UserId, ct);
        foreach (var session in activeSessions.Where(s => s.Id != cmd.SessionId))
            session.Revoke();

        await unitOfWork.CommitAsync(ct);

        logger.LogInformation("Senha alterada. UserId={UserId}", cmd.UserId);
    }
}
