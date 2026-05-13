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
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IEmailService emailService,
    IUnitOfWork unitOfWork,
    ILogger<RegisterDemoHandler> logger
) : IRequestHandler<RegisterDemoCommand, RegisterDemoResult>
{
    public async Task<RegisterDemoResult> Handle(RegisterDemoCommand cmd, CancellationToken ct)
    {
        if (await users.ExistsByEmailAsync(cmd.Email, ct))
            throw new DomainException("E-mail já cadastrado.");

        // Gera senha temporária segura de 16 caracteres
        var tempPassword = tokenService.GenerateRefreshToken()[..16];
        var passwordHash = passwordHasher.Hash(tempPassword);

        var user = User.CreateDemo(cmd.Email, passwordHash, PasswordAlgorithm.Argon2id);
        await users.AddAsync(user, ct);
        await unitOfWork.CommitAsync(ct);

        await emailService.SendTemporaryPasswordAsync(user.Email, tempPassword, ct);

        logger.LogInformation("Conta demo criada. UserId={UserId}", user.Id);

        return new RegisterDemoResult("Conta criada. Verifique seu e-mail para acessar.");
    }
}
