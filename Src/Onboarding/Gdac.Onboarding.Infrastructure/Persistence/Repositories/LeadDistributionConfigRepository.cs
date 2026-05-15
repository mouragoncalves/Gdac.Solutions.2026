using Gdac.Onboarding.Domain.Entities;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Onboarding.Infrastructure.Persistence.Repositories;

public class LeadDistributionConfigRepository(OnboardingDbContext db) : ILeadDistributionConfigRepository
{
    public Task<LeadDistributionConfig?> GetAsync(CancellationToken ct = default)
        => db.LeadDistributionConfigs.FirstOrDefaultAsync(ct);

    public async Task AddAsync(LeadDistributionConfig config, CancellationToken ct = default)
        => await db.LeadDistributionConfigs.AddAsync(config, ct);

    public void Update(LeadDistributionConfig config)
        => db.LeadDistributionConfigs.Update(config);
}
