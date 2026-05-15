using Gdac.Content.Domain.Enums;
using MediatR;

namespace Gdac.Content.Application.Features.Integrations.Commands.UpdateIntegration;

public record UpdateIntegrationCommand(
    Guid                Id,
    string              Name,
    IntegrationCategory Category,
    string              LogoUrl,
    string              Description,
    string?             ExternalUrl) : IRequest;
