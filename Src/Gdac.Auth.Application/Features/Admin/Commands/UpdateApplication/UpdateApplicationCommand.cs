using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.UpdateApplication;

public record UpdateApplicationCommand(Guid ApplicationId, string Name, bool IsActive) : IRequest;
