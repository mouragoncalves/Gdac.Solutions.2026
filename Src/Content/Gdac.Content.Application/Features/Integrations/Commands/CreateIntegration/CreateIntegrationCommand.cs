using Gdac.Content.Domain.Enums;
using MediatR;

namespace Gdac.Content.Application.Features.Integrations.Commands.CreateIntegration;

public record CreateIntegrationCommand(
    string              Name,
    IntegrationCategory Category,
    string              LogoUrl,
    string              Description,
    string?             ExternalUrl) : IRequest<Guid>;
