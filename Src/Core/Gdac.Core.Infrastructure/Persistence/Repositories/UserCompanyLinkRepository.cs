using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Core.Infrastructure.Persistence.Repositories;

public class UserCompanyLinkRepository(CoreDbContext db) : IUserCompanyLinkRepository
{
    public Task<UserCompanyLink?> GetAsync(Guid userId, Guid companyId, CancellationToken ct = default) =>
        db.UserCompanyLinks.FirstOrDefaultAsync(l => l.UserId == userId && l.CompanyId == companyId, ct);

    public async Task<IReadOnlyList<UserCompanyLink>> GetByCompanyAsync(Guid companyId, CancellationToken ct = default) =>
        await db.UserCompanyLinks.Include(l => l.User)
            .Where(l => l.CompanyId == companyId)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<UserCompanyLink>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await db.UserCompanyLinks.Include(l => l.Company)
            .Where(l => l.UserId == userId)
            .ToListAsync(ct);

    public async Task AddAsync(UserCompanyLink link, CancellationToken ct = default) =>
        await db.UserCompanyLinks.AddAsync(link, ct);

    public void Update(UserCompanyLink link) =>
        db.UserCompanyLinks.Update(link);
}
