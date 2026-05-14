using Gdac.Auth.Domain.Exceptions;
using DomainSession = Gdac.Auth.Domain.Entities.Session;
using Gdac.Auth.Domain.Interfaces.Repositories;
using Gdac.Auth.Domain.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gdac.Auth.Application.Features.Auth.Commands.Login;

public class LoginHandler(
    IUserRepository users,
    IApplicationRepository applications,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ISessionRepository sessions,
    IUnitOfWork unitOfWork,
    ILogger<LoginHandler> logger
) : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var app = await applications.FindByClientIdAsync(cmd.ClientId, ct)
            ?? throw new UnauthorizedException("Aplicação não autorizada.");

        if (!app.IsActive)
            throw new UnauthorizedException("Aplicação não autorizada.");

        if (!passwordHasher.Verify(cmd.ClientSecret, app.ClientSecretHash, Domain.Enums.PasswordAlgorithm.Argon2id))
            throw new UnauthorizedException("Aplicação não autorizada.");

        var user = await users.FindByEmailAsync(cmd.Email, ct)
            ?? throw new UnauthorizedException("Usuário ou senha inválidos.");

        if (user.IsLockedOut())
            throw new UnauthorizedException("Conta temporariamente bloqueada. Tente novamente mais tarde.");

        if (!user.IsActive)
            throw new UnauthorizedException("Usuário inativo.");

        if (!passwordHasher.Verify(cmd.Password, user.PasswordHash, user.PasswordAlgorithm))
        {
            user.RegisterFailedLogin();
            users.Update(user);
            await unitOfWork.CommitAsync(ct);

            logger.LogWarning("Falha de login para {Email} a partir de {Ip}", cmd.Email, cmd.IpAddress);
            throw new UnauthorizedException("Usuário ou senha inválidos.");
        }

        var hasAccess = await users.HasAccessToApplicationAsync(user.Id, app.Id, ct);
        if (!hasAccess)
            throw new UnauthorizedException("Usuário sem acesso a esta aplicação.");

        user.ResetLoginAttempts();

        if (passwordHasher.NeedsUpgrade(user.PasswordAlgorithm))
        {
            var newHash = passwordHasher.Hash(cmd.Password);
            user.UpdatePassword(newHash, Domain.Enums.PasswordAlgorithm.Argon2id);
        }

        var refreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenHash = tokenService.HashRefreshToken(refreshToken);

        var session = DomainSession.Create(user.Id, app.Id, refreshTokenHash, cmd.IpAddress, cmd.DeviceInfo);
        await sessions.AddAsync(session, ct);

        users.Update(user);
        await unitOfWork.CommitAsync(ct);

        var accessToken = tokenService.GenerateAccessToken(user.Id, session.Id);

        logger.LogInformation("Login realizado. UserId={UserId} SessionId={SessionId} AppId={AppId} Ip={Ip}",
            user.Id, session.Id, app.Id, cmd.IpAddress);

        return new LoginResult(accessToken, refreshToken, 900, session.Id, user.MustChangePassword);
    }
}
