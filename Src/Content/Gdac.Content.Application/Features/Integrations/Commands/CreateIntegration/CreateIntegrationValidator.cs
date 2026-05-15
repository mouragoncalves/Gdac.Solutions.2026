using FluentValidation;

namespace Gdac.Content.Application.Features.Integrations.Commands.CreateIntegration;

public class CreateIntegrationValidator : AbstractValidator<CreateIntegrationCommand>
{
    public CreateIntegrationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.LogoUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Description).NotEmpty();
    }
}
