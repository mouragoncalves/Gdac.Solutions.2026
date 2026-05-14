using Gdac.Core.Domain.Interfaces.Repositories;

namespace Gdac.Core.Infrastructure.Persistence.Repositories;

public class UnitOfWork(CoreDbContext db) : IUnitOfWork
{
    public Task<int> CommitAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
