using MediatR;

namespace Gdac.Auth.Application.Features.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(
    Guid UserId,
    Guid SessionId,
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
) : IRequest;
