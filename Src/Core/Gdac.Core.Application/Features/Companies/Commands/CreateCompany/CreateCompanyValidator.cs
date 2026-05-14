using FluentValidation;

namespace Gdac.Core.Application.Features.Companies.Commands.CreateCompany;

public class CreateCompanyValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Cnpj).Length(14).When(x => x.Cnpj != null);
        RuleFor(x => x.Email).EmailAddress().When(x => x.Email != null);
    }
}
