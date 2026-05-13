using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Session.Queries.GetMe;

public class GetMeHandler(
    IUserRepository users,
    ISessionRepository sessions
) : IRequestHandler<GetMeQuery, GetMeResult>
{
    public async Task<GetMeResult> Handle(GetMeQuery query, CancellationToken ct)
    {
        var user = await users.FindByIdAsync(query.UserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), query.UserId);

        var session = await sessions.FindByIdAsync(query.SessionId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Session), query.SessionId);

        return new GetMeResult(
            user.Id,
            user.Email,
            user.MustChangePassword,
            session.Id,
            session.CreatedAt,
            session.AbsoluteExpiration
        );
    }
}
