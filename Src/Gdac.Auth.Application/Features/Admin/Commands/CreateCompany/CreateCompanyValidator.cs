using FluentValidation;

namespace Gdac.Auth.Application.Features.Admin.Commands.CreateCompany;

public class CreateCompanyValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.ExternalId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
