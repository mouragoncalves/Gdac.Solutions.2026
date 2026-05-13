using Gdac.Auth.Domain.Entities;

namespace Gdac.Auth.Domain.Interfaces.Repositories;

public interface ISessionRepository
{
    Task<Session?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Session>> FindActiveByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Session session, CancellationToken ct = default);
    void Update(Session session);
    Task RevokeAllByUserIdAsync(Guid userId, CancellationToken ct = default);
}
