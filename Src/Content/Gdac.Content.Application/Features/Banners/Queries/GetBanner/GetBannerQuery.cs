using MediatR;

namespace Gdac.Content.Application.Features.Banners.Queries.GetBanner;

public record GetBannerQuery(Guid Id) : IRequest<BannerResult>;

public record BannerResult(
    Guid    Id,
    string  Title,
    string? Subtitle,
    string  ImageUrl,
    string  CtaText,
    string  CtaUrl,
    string? SecondaryCtaText,
    string? SecondaryCtaUrl,
    bool    IsActive,
    int     DisplayOrder,
    Guid?   PartnerId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
