namespace Gdac.Core.Domain.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
