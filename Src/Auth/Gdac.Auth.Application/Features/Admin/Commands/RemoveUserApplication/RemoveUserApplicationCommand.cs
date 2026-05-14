using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.RemoveUserApplication;

public record RemoveUserApplicationCommand(Guid UserId, Guid ApplicationId) : IRequest;
