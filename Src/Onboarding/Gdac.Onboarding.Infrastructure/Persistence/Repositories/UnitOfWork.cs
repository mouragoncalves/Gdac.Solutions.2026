using Gdac.Onboarding.Domain.Interfaces.Repositories;

namespace Gdac.Onboarding.Infrastructure.Persistence.Repositories;

public class UnitOfWork(OnboardingDbContext db) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken ct = default) =>
        await db.SaveChangesAsync(ct);
}
