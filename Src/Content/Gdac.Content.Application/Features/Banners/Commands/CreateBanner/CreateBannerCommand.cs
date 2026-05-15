using MediatR;

namespace Gdac.Content.Application.Features.Banners.Commands.CreateBanner;

public record CreateBannerCommand(
    string  Title,
    string? Subtitle,
    string  ImageUrl,
    string  CtaText,
    string  CtaUrl,
    string? SecondaryCtaText,
    string? SecondaryCtaUrl,
    Guid?   PartnerId) : IRequest<Guid>;
