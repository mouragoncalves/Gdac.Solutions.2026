namespace Gdac.Onboarding.Domain.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken ct = default);
}
