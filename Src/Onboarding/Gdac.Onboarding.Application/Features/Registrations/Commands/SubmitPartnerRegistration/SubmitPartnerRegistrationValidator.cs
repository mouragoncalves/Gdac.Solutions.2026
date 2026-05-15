using FluentValidation;

namespace Gdac.Onboarding.Application.Features.Registrations.Commands.SubmitPartnerRegistration;

public class SubmitPartnerRegistrationValidator : AbstractValidator<SubmitPartnerRegistrationCommand>
{
    public SubmitPartnerRegistrationValidator()
    {
        RuleFor(x => x.ContactName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.ContactPhone).MaximumLength(20).When(x => x.ContactPhone != null);
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TradeName).MaximumLength(200).When(x => x.TradeName != null);
        RuleFor(x => x.Cnpj)
            .NotEmpty()
            .Length(14)
            .Matches(@"^\d{14}$").WithMessage("CNPJ deve conter exatamente 14 dígitos numéricos.");
        RuleFor(x => x.State).MaximumLength(2).When(x => x.State != null);
        RuleFor(x => x.City).MaximumLength(100).When(x => x.City != null);
        RuleFor(x => x.Segment).IsInEnum().When(x => x.Segment != null);
        RuleFor(x => x.SizeCategory).IsInEnum().When(x => x.SizeCategory != null);
    }
}
