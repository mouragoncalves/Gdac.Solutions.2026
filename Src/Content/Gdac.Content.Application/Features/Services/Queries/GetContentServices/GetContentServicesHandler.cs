using Gdac.Content.Application.Features.Services.Queries.GetContentService;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Services.Queries.GetContentServices;

public class GetContentServicesHandler(IContentServiceRepository repo)
    : IRequestHandler<GetContentServicesQuery, IReadOnlyList<ContentServiceResult>>
{
    public async Task<IReadOnlyList<ContentServiceResult>> Handle(GetContentServicesQuery request, CancellationToken ct)
    {
        var list = await repo.GetActiveAsync(ct);
        return list.Select(GetContentServiceHandler.MapToResult).ToList();
    }
}
