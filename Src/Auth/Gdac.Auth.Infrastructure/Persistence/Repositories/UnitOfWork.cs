using Gdac.Auth.Domain.Interfaces.Repositories;

namespace Gdac.Auth.Infrastructure.Persistence.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> CommitAsync(CancellationToken ct = default) =>
        context.SaveChangesAsync(ct);
}
