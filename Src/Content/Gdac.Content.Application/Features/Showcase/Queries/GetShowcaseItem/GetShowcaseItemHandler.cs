using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Showcase.Queries.GetShowcaseItem;

public class GetShowcaseItemHandler(IShowcaseItemRepository repo)
    : IRequestHandler<GetShowcaseItemQuery, ShowcaseItemResult>
{
    public async Task<ShowcaseItemResult> Handle(GetShowcaseItemQuery request, CancellationToken ct)
    {
        var item = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(ShowcaseItem), request.Id);

        return new ShowcaseItemResult(
            item.Id, item.Type, item.CoreCompanyId, item.Name, item.LogoUrl,
            item.IsActive, item.DisplayOrder, item.CreatedAt, item.UpdatedAt);
    }
}
