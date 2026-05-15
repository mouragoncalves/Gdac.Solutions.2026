using Gdac.Content.Domain.Interfaces.Repositories;

namespace Gdac.Content.Infrastructure.Persistence.Repositories;

public class UnitOfWork(ContentDbContext db) : IUnitOfWork
{
    public Task<int> CommitAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}
