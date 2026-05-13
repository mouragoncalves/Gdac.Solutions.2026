using Gdac.Auth.Domain.Entities;
using Gdac.Auth.Domain.Interfaces.Repositories;
using Gdac.Auth.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gdac.Auth.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordHandler(
    IUserRepository users,
    IPasswordResetTokenRepository resetTokens,
    ITokenService tokenService,
    IEmailService emailService,
    IUnitOfWork unitOfWork,
    ILogger<ForgotPasswordHandler> logger
) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand cmd, CancellationToken ct)
    {
        // Nunca revelar se o e-mail existe ou não
        var user = await users.FindByEmailAsync(cmd.Email, ct);
        if (user is null)
        {
            logger.LogInformation("Solicitação de reset para e-mail não cadastrado.");
            return;
        }

        await resetTokens.InvalidateAllByUserIdAsync(user.Id, ct);

        var rawToken = tokenService.GenerateRefreshToken();
        var tokenHash = tokenService.HashRefreshToken(rawToken);

        var resetToken = PasswordResetToken.Create(user.Id, tokenHash);
        await resetTokens.AddAsync(resetToken, ct);
        await unitOfWork.CommitAsync(ct);

        await emailService.SendPasswordResetAsync(user.Email, rawToken, ct);

        logger.LogInformation("Token de reset gerado. UserId={UserId}", user.Id);
    }
}
