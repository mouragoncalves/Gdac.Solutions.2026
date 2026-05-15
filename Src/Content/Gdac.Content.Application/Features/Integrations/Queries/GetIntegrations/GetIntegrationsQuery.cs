using Gdac.Content.Application.Features.Integrations.Queries.GetIntegration;
using Gdac.Content.Domain.Enums;
using MediatR;

namespace Gdac.Content.Application.Features.Integrations.Queries.GetIntegrations;

public record GetIntegrationsQuery(IntegrationCategory? Category = null) : IRequest<IReadOnlyList<IntegrationResult>>;
