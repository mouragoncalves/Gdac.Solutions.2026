using MediatR;

namespace Gdac.Content.Application.Features.Banners.Commands.UpdateBanner;

public record UpdateBannerCommand(
    Guid    Id,
    string  Title,
    string? Subtitle,
    string  ImageUrl,
    string  CtaText,
    string  CtaUrl,
    string? SecondaryCtaText,
    string? SecondaryCtaUrl) : IRequest;
