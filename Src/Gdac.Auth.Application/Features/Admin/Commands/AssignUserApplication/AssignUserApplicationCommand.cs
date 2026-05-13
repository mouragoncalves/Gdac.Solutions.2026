using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.AssignUserApplication;

public record AssignUserApplicationCommand(Guid UserId, Guid ApplicationId) : IRequest;
