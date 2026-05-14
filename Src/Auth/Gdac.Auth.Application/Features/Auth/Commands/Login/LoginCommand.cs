using MediatR;

namespace Gdac.Auth.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password,
    string ClientId,
    string ClientSecret,
    string IpAddress,
    string DeviceInfo
) : IRequest<LoginResult>;

public record LoginResult(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    Guid SessionId,
    bool ForceChangePassword
);
