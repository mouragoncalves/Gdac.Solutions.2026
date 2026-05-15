using Gdac.Onboarding.Domain.Enums;
using MediatR;

namespace Gdac.Onboarding.Application.Features.LeadDistribution.Commands.UpdateLeadDistributionConfig;

public record UpdateLeadDistributionConfigCommand(
    LeadDistributionMode Mode,
    Guid? DefaultPartnerId,
    Guid UpdatedBy) : IRequest;
