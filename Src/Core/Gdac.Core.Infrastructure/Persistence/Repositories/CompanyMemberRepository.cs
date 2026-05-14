using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Core.Infrastructure.Persistence.Repositories;

public class CompanyMemberRepository(CoreDbContext db) : ICompanyMemberRepository
{
    public async Task<IReadOnlyList<CompanyMember>> GetByCompanyIdAsync(Guid companyId, CancellationToken ct = default) =>
        await db.CompanyMembers.Where(m => m.CompanyId == companyId)
            .OrderBy(m => m.PersonName).ToListAsync(ct);

    public Task<CompanyMember?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.CompanyMembers.FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task AddAsync(CompanyMember member, CancellationToken ct = default) =>
        await db.CompanyMembers.AddAsync(member, ct);

    public async Task AddRangeAsync(IEnumerable<CompanyMember> members, CancellationToken ct = default) =>
        await db.CompanyMembers.AddRangeAsync(members, ct);

    public void Remove(CompanyMember member) =>
        db.CompanyMembers.Remove(member);

    public async Task RemoveAllByCompanyIdAsync(Guid companyId, CancellationToken ct = default)
    {
        var members = await db.CompanyMembers.Where(m => m.CompanyId == companyId).ToListAsync(ct);
        db.CompanyMembers.RemoveRange(members);
    }
}
