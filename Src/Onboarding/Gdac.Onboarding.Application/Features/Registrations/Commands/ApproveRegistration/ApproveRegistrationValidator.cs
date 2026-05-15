using FluentValidation;

namespace Gdac.Onboarding.Application.Features.Registrations.Commands.ApproveRegistration;

public class ApproveRegistrationValidator : AbstractValidator<ApproveRegistrationCommand>
{
    public ApproveRegistrationValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ReviewedBy).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(1000).When(x => x.Notes != null);
    }
}
