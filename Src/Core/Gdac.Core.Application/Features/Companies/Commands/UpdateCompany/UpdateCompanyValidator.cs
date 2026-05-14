using FluentValidation;

namespace Gdac.Core.Application.Features.Companies.Commands.UpdateCompany;

public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Cnpj).Length(14).When(x => x.Cnpj != null);
        RuleFor(x => x.Email).EmailAddress().When(x => x.Email != null);
    }
}
