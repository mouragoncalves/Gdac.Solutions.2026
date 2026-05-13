using Gdac.Auth.Domain.Entities;
using Gdac.Auth.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Auth.Infrastructure.Persistence.Repositories;

public class SessionRepository(AppDbContext context) : ISessionRepository
{
    public Task<Session?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Sessions.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<Session>> FindActiveByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.Sessions
            .Where(s => s.UserId == userId && !s.IsRevoked)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Session session, CancellationToken ct = default) =>
        await context.Sessions.AddAsync(session, ct);

    public void Update(Session session) =>
        context.Sessions.Update(session);

    public async Task RevokeAllByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        await context.Sessions
            .Where(s => s.UserId == userId && !s.IsRevoked)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsRevoked, true), ct);
    }
}
