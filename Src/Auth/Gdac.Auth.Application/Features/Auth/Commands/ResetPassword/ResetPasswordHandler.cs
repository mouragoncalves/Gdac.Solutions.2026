using Gdac.Auth.Domain.Enums;
using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using Gdac.Auth.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gdac.Auth.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordHandler(
    IPasswordResetTokenRepository resetTokens,
    IUserRepository users,
    ISessionRepository sessions,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    ILogger<ResetPasswordHandler> logger
) : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand cmd, CancellationToken ct)
    {
        var tokenHash = tokenService.HashRefreshToken(cmd.Token);
        var resetToken = await resetTokens.FindByTokenHashAsync(tokenHash, ct)
            ?? throw new UnauthorizedException("Token inválido ou expirado.");

        if (!resetToken.IsValid())
            throw new UnauthorizedException("Token inválido ou expirado.");

        var user = await users.FindByIdAsync(resetToken.UserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), resetToken.UserId);

        var newHash = passwordHasher.Hash(cmd.NewPassword);
        user.UpdatePassword(newHash, PasswordAlgorithm.Argon2id);
        users.Update(user);

        resetToken.MarkAsUsed();
        resetTokens.Update(resetToken);

        await sessions.RevokeAllByUserIdAsync(user.Id, ct);
        await unitOfWork.CommitAsync(ct);

        logger.LogInformation("Senha redefinida via token. UserId={UserId}", user.Id);
    }
}
