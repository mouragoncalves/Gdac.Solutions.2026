using MediatR;

namespace Gdac.Auth.Application.Features.Auth.Commands.Refresh;

public record RefreshCommand(
    string AccessToken,
    string RefreshToken,
    string IpAddress
) : IRequest<RefreshResult>;

public record RefreshResult(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    Guid SessionId
);
