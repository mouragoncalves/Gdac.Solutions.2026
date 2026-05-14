using Gdac.Core.Domain.Entities;

namespace Gdac.Core.Domain.Interfaces.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<UserProfile>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(UserProfile profile, CancellationToken ct = default);
    void Update(UserProfile profile);
}
