using Gdac.Onboarding.Domain.Enums;
using MediatR;

namespace Gdac.Onboarding.Application.Features.LeadDistribution.Queries.GetLeadDistributionConfig;

public record GetLeadDistributionConfigQuery : IRequest<LeadDistributionConfigResult?>;

public record LeadDistributionConfigResult(
    Guid Id,
    LeadDistributionMode Mode,
    Guid? DefaultPartnerId,
    DateTime UpdatedAt,
    Guid UpdatedBy);
