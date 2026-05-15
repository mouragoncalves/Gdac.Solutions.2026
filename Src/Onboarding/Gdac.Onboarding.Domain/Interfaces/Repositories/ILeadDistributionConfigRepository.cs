using Gdac.Onboarding.Domain.Entities;

namespace Gdac.Onboarding.Domain.Interfaces.Repositories;

public interface ILeadDistributionConfigRepository
{
    Task<LeadDistributionConfig?> GetAsync(CancellationToken ct = default);
    Task AddAsync(LeadDistributionConfig config, CancellationToken ct = default);
    void Update(LeadDistributionConfig config);
}
