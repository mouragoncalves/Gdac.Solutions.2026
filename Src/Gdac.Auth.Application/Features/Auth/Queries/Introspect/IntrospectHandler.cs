using Gdac.Auth.Domain.Interfaces.Repositories;
using Gdac.Auth.Domain.Interfaces.Services;
using MediatR;

namespace Gdac.Auth.Application.Features.Auth.Queries.Introspect;

public class IntrospectHandler(
    IApplicationRepository applications,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ISessionRepository sessions
) : IRequestHandler<IntrospectQuery, IntrospectResult>
{
    private static readonly TimeSpan IdleTimeout = TimeSpan.FromHours(1);
    private static readonly IntrospectResult Inactive = new(false, null, null, null);

    public async Task<IntrospectResult> Handle(IntrospectQuery query, CancellationToken ct)
    {
        var app = await applications.FindByClientIdAsync(query.ClientId, ct);
        if (app is null || !app.IsActive)
            return Inactive;

        if (!passwordHasher.Verify(query.ClientSecret, app.ClientSecretHash, Domain.Enums.PasswordAlgorithm.Argon2id))
            return Inactive;

        var sessionId = tokenService.ExtractSessionId(query.AccessToken);
        if (sessionId is null)
            return Inactive;

        var session = await sessions.FindByIdAsync(sessionId.Value, ct);
        if (session is null || !session.IsActive(IdleTimeout))
            return Inactive;

        return new IntrospectResult(true, session.UserId, session.Id, null);
    }
}
