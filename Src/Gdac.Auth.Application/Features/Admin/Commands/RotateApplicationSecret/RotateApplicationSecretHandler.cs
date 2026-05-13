using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using Gdac.Auth.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gdac.Auth.Application.Features.Admin.Commands.RotateApplicationSecret;

public class RotateApplicationSecretHandler(
    IApplicationRepository applications,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    ILogger<RotateApplicationSecretHandler> logger
) : IRequestHandler<RotateApplicationSecretCommand, RotateApplicationSecretResult>
{
    public async Task<RotateApplicationSecretResult> Handle(RotateApplicationSecretCommand cmd, CancellationToken ct)
    {
        var app = await applications.FindByIdAsync(cmd.ApplicationId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Application), cmd.ApplicationId);

        var newSecret = tokenService.GenerateRefreshToken();
        app.RotateSecret(passwordHasher.Hash(newSecret));

        applications.Update(app);
        await unitOfWork.CommitAsync(ct);

        logger.LogInformation("Secret rotacionado. AppId={AppId}", app.Id);
        return new RotateApplicationSecretResult(newSecret);
    }
}
