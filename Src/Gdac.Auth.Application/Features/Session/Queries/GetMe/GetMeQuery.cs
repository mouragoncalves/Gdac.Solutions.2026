using MediatR;

namespace Gdac.Auth.Application.Features.Session.Queries.GetMe;

public record GetMeQuery(Guid UserId, Guid SessionId) : IRequest<GetMeResult>;

public record GetMeResult(
    Guid Id,
    string Email,
    bool MustChangePassword,
    Guid SessionId,
    DateTime SessionCreatedAt,
    DateTime SessionExpiresAt
);
