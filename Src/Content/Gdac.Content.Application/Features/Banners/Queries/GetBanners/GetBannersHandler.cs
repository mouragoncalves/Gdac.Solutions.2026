using Gdac.Content.Application.Features.Banners.Queries.GetBanner;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Banners.Queries.GetBanners;

public class GetBannersHandler(IBannerRepository repo)
    : IRequestHandler<GetBannersQuery, IReadOnlyList<BannerResult>>
{
    public async Task<IReadOnlyList<BannerResult>> Handle(GetBannersQuery request, CancellationToken ct)
    {
        var list = await repo.GetActiveAsync(request.PartnerId, ct);
        return list.Select(b => new BannerResult(
            b.Id, b.Title, b.Subtitle,
            b.ImageUrl, b.CtaText, b.CtaUrl,
            b.SecondaryCtaText, b.SecondaryCtaUrl,
            b.IsActive, b.DisplayOrder, b.PartnerId,
            b.CreatedAt, b.UpdatedAt)).ToList();
    }
}
