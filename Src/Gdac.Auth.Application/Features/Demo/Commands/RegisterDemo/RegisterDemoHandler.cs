using Gdac.Auth.Domain.Entities;
using Gdac.Auth.Domain.Enums;
using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using Gdac.Auth.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gdac.Auth.Application.Features.Demo.Commands.RegisterDemo;

public class RegisterDemoHandler(
    IUserRepository users,
    IApplicationRepository applications,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IEmailService emailService,
    IUnitOfWork unitOfWork,
    ILogger<RegisterDemoHandler> logger
) : IRequestHandler<RegisterDemoCommand, RegisterDemoResult>
{
    public async Task<RegisterDemoResult> Handle(RegisterDemoCommand cmd, CancellationToken ct)
    {
        var app = await applications.FindByClientIdAsync(cmd.ClientId, ct);
        if (app is null || !app.IsActive)
            throw new UnauthorizedException("Aplicação inválida.");

        if (!passwordHasher.Verify(cmd.ClientSecret, app.ClientSecretHash, PasswordAlgorithm.Argon2id))
            throw new UnauthorizedException("Aplicação inválida.");

        if (await users.ExistsByEmailAsync(cmd.Email, ct))
            throw new DomainException("E-mail já cadastrado.");

        var tempPassword = tokenService.GenerateRefreshToken()[..16];
        var passwordHash = passwordHasher.Hash(tempPassword);

        var user = User.CreateDemo(cmd.Email, passwordHash, PasswordAlgorithm.Argon2id);
        await users.AddAsync(user, ct);
        await users.AssignApplicationAsync(user.Id, app.Id, ct);
        await unitOfWork.CommitAsync(ct);

        await emailService.SendTemporaryPasswordAsync(user.Email, tempPassword, ct);

        logger.LogInformation("Conta demo criada. UserId={UserId} AppId={AppId}", user.Id, app.Id);

        return new RegisterDemoResult("Conta criada. Verifique seu e-mail para acessar.");
    }
}
