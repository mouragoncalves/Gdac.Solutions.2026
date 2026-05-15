using MediatR;

namespace Gdac.Content.Application.Features.Integrations.Commands.SetIntegrationActive;

public record SetIntegrationActiveCommand(Guid Id, bool IsActive) : IRequest;
