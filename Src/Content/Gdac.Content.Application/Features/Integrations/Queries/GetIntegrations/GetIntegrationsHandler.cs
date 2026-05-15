using Gdac.Content.Application.Features.Integrations.Queries.GetIntegration;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Integrations.Queries.GetIntegrations;

public class GetIntegrationsHandler(IIntegrationRepository repo)
    : IRequestHandler<GetIntegrationsQuery, IReadOnlyList<IntegrationResult>>
{
    public async Task<IReadOnlyList<IntegrationResult>> Handle(GetIntegrationsQuery request, CancellationToken ct)
    {
        var list = await repo.GetActiveAsync(request.Category, ct);
        return list.Select(i => new IntegrationResult(
            i.Id, i.Name, i.Category, i.LogoUrl, i.Description,
            i.ExternalUrl, i.IsActive, i.DisplayOrder, i.CreatedAt, i.UpdatedAt)).ToList();
    }
}
