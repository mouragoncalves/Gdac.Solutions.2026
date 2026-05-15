using FluentValidation;

namespace Gdac.Content.Application.Features.Products.Commands.CreateProduct;

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.PrecoRevenda).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PrecoSugeridoFinal).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DescontoSemestral).InclusiveBetween(0, 100);
        RuleFor(x => x.DescontoAnual).InclusiveBetween(0, 100);
    }
}
