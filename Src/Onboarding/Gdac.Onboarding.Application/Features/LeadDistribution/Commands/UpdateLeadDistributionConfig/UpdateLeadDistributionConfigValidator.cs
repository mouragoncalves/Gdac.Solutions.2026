using FluentValidation;
using Gdac.Onboarding.Domain.Enums;

namespace Gdac.Onboarding.Application.Features.LeadDistribution.Commands.UpdateLeadDistributionConfig;

public class UpdateLeadDistributionConfigValidator : AbstractValidator<UpdateLeadDistributionConfigCommand>
{
    public UpdateLeadDistributionConfigValidator()
    {
        RuleFor(x => x.Mode).IsInEnum();
        RuleFor(x => x.UpdatedBy).NotEmpty();
        RuleFor(x => x.DefaultPartnerId)
            .NotEmpty()
            .When(x => x.Mode == LeadDistributionMode.RevendaPadrao)
            .WithMessage("DefaultPartnerId é obrigatório para o modo RevendaPadrao.");
    }
}
