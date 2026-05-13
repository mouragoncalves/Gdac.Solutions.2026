using MediatR;

namespace Gdac.Auth.Application.Features.Auth.Queries.Introspect;

public record IntrospectQuery(
    string AccessToken,
    string ClientId,
    string ClientSecret
) : IRequest<IntrospectResult>;

public record IntrospectResult(
    bool Active,
    Guid? Sub,
    Guid? Sid,
    long? Exp
);
