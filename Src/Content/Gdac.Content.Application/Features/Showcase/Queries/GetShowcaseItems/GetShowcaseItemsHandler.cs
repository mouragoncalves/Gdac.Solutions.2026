using Gdac.Content.Application.Features.Showcase.Queries.GetShowcaseItem;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Showcase.Queries.GetShowcaseItems;

public class GetShowcaseItemsHandler(IShowcaseItemRepository repo)
    : IRequestHandler<GetShowcaseItemsQuery, IReadOnlyList<ShowcaseItemResult>>
{
    public async Task<IReadOnlyList<ShowcaseItemResult>> Handle(GetShowcaseItemsQuery request, CancellationToken ct)
    {
        var list = await repo.GetActiveAsync(request.Type, ct);
        return list.Select(item => new ShowcaseItemResult(
            item.Id, item.Type, item.CoreCompanyId, item.Name, item.LogoUrl,
            item.IsActive, item.DisplayOrder, item.CreatedAt, item.UpdatedAt)).ToList();
    }
}
