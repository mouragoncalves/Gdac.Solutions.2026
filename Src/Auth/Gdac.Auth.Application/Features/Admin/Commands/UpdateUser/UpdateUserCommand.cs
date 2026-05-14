using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.UpdateUser;

public record UpdateUserCommand(Guid UserId, bool IsActive, bool MustChangePassword) : IRequest;
