using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.DeactivateUser;

public record DeactivateUserCommand(Guid UserId) : IRequest;
