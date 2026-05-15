using Gdac.Onboarding.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Onboarding.Application.Features.LeadDistribution.Queries.GetLeadDistributionConfig;

public class GetLeadDistributionConfigHandler(ILeadDistributionConfigRepository configRepo)
    : IRequestHandler<GetLeadDistributionConfigQuery, LeadDistributionConfigResult?>
{
    public async Task<LeadDistributionConfigResult?> Handle(GetLeadDistributionConfigQuery request, CancellationToken ct)
    {
        var config = await configRepo.GetAsync(ct);
        if (config is null) return null;

        return new LeadDistributionConfigResult(
            config.Id, config.Mode, config.DefaultPartnerId,
            config.UpdatedAt, config.UpdatedBy);
    }
}
