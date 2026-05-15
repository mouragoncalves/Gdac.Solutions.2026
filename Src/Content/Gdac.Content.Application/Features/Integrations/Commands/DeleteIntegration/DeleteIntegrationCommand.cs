using MediatR;

namespace Gdac.Content.Application.Features.Integrations.Commands.DeleteIntegration;

public record DeleteIntegrationCommand(Guid Id) : IRequest;
