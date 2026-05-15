using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Banners.Queries.GetBanner;

public class GetBannerHandler(IBannerRepository repo)
    : IRequestHandler<GetBannerQuery, BannerResult>
{
    public async Task<BannerResult> Handle(GetBannerQuery request, CancellationToken ct)
    {
        var banner = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Banner), request.Id);

        return new BannerResult(
            banner.Id, banner.Title, banner.Subtitle,
            banner.ImageUrl, banner.CtaText, banner.CtaUrl,
            banner.SecondaryCtaText, banner.SecondaryCtaUrl,
            banner.IsActive, banner.DisplayOrder, banner.PartnerId,
            banner.CreatedAt, banner.UpdatedAt);
    }
}
