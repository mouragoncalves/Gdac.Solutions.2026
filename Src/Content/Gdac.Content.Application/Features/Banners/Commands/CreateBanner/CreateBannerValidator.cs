using FluentValidation;

namespace Gdac.Content.Application.Features.Banners.Commands.CreateBanner;

public class CreateBannerValidator : AbstractValidator<CreateBannerCommand>
{
    public CreateBannerValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.CtaText).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CtaUrl).NotEmpty().MaximumLength(500);
    }
}
