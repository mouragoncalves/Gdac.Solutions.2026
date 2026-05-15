using Gdac.Content.Application.Features.Banners.Queries.GetBanner;
using MediatR;

namespace Gdac.Content.Application.Features.Banners.Queries.GetBanners;

public record GetBannersQuery(Guid? PartnerId = null) : IRequest<IReadOnlyList<BannerResult>>;
