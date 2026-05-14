using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Core.Infrastructure.Persistence.Repositories;

public class UserProfileRepository(CoreDbContext db) : IUserProfileRepository
{
    public Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.UserProfiles.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<IReadOnlyList<UserProfile>> GetAllAsync(CancellationToken ct = default) =>
        await db.UserProfiles.OrderBy(u => u.FullName).ToListAsync(ct);

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        db.UserProfiles.AnyAsync(u => u.Id == id, ct);

    public async Task AddAsync(UserProfile profile, CancellationToken ct = default) =>
        await db.UserProfiles.AddAsync(profile, ct);

    public void Update(UserProfile profile) =>
        db.UserProfiles.Update(profile);
}
