using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using Gdac.Auth.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gdac.Auth.Application.Features.Admin.Commands.CreateApplication;

public class CreateApplicationHandler(
    IApplicationRepository applications,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    ILogger<CreateApplicationHandler> logger
) : IRequestHandler<CreateApplicationCommand, CreateApplicationResult>
{
    public async Task<CreateApplicationResult> Handle(CreateApplicationCommand cmd, CancellationToken ct)
    {
        var existing = await applications.FindByClientIdAsync(cmd.ClientId, ct);
        if (existing is not null)
            throw new DomainException("ClientId já está em uso.");

        var secret = tokenService.GenerateRefreshToken();
        var secretHash = passwordHasher.Hash(secret);

        var app = Domain.Entities.Application.Create(cmd.Name, cmd.ClientId, secretHash);
        await applications.AddAsync(app, ct);
        await unitOfWork.CommitAsync(ct);

        logger.LogInformation("Aplicação criada. AppId={AppId} ClientId={ClientId}", app.Id, app.ClientId);

        return new CreateApplicationResult(app.Id, app.Name, app.ClientId, secret);
    }
}
