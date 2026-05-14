using Gdac.Auth.Domain.Entities;
using Gdac.Auth.Domain.Enums;
using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using Gdac.Auth.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gdac.Auth.Application.Features.Admin.Commands.CreateUser;

public class CreateUserHandler(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    ILogger<CreateUserHandler> logger
) : IRequestHandler<CreateUserCommand, CreateUserResult>
{
    public async Task<CreateUserResult> Handle(CreateUserCommand cmd, CancellationToken ct)
    {
        if (await users.ExistsByEmailAsync(cmd.Email, ct))
            throw new DomainException("E-mail já cadastrado.");

        var tempPassword = tokenService.GenerateRefreshToken()[..16];
        var hash = passwordHasher.Hash(tempPassword);

        var user = cmd.MustChangePassword
            ? User.CreateDemo(cmd.Email, hash, PasswordAlgorithm.Argon2id)
            : User.Create(cmd.Email, hash, PasswordAlgorithm.Argon2id);

        await users.AddAsync(user, ct);
        await unitOfWork.CommitAsync(ct);

        logger.LogInformation("Usuário criado via admin. UserId={UserId}", user.Id);

        return new CreateUserResult(user.Id, user.Email, tempPassword);
    }
}
