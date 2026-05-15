using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Integrations.Queries.GetIntegration;

public class GetIntegrationHandler(IIntegrationRepository repo)
    : IRequestHandler<GetIntegrationQuery, IntegrationResult>
{
    public async Task<IntegrationResult> Handle(GetIntegrationQuery request, CancellationToken ct)
    {
        var i = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Integration), request.Id);

        return new IntegrationResult(
            i.Id, i.Name, i.Category, i.LogoUrl, i.Description,
            i.ExternalUrl, i.IsActive, i.DisplayOrder, i.CreatedAt, i.UpdatedAt);
    }
}
