using Gdac.Auth.Domain.Entities;

namespace Gdac.Auth.Domain.Interfaces.Repositories;

public interface IApplicationRepository
{
    Task<Application?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task<Application?> FindByClientIdAsync(string clientId, CancellationToken ct = default);
    Task<IReadOnlyList<Application>> FindByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Application application, CancellationToken ct = default);
    void Update(Application application);
}
