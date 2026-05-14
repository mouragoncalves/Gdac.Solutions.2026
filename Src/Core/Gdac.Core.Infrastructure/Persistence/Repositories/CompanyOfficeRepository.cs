using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Core.Infrastructure.Persistence.Repositories;

public class CompanyOfficeRepository(CoreDbContext db) : ICompanyOfficeRepository
{
    public async Task<IReadOnlyList<CompanyOffice>> GetByCompanyIdAsync(Guid companyId, CancellationToken ct = default) =>
        await db.CompanyOffices.Where(o => o.CompanyId == companyId)
            .OrderByDescending(o => o.IsHead).ThenBy(o => o.TaxId).ToListAsync(ct);

    public Task<CompanyOffice?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.CompanyOffices.FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task AddAsync(CompanyOffice office, CancellationToken ct = default) =>
        await db.CompanyOffices.AddAsync(office, ct);

    public async Task AddRangeAsync(IEnumerable<CompanyOffice> offices, CancellationToken ct = default) =>
        await db.CompanyOffices.AddRangeAsync(offices, ct);

    public void Remove(CompanyOffice office) =>
        db.CompanyOffices.Remove(office);

    public async Task RemoveAllByCompanyIdAsync(Guid companyId, CancellationToken ct = default)
    {
        var offices = await db.CompanyOffices.Where(o => o.CompanyId == companyId).ToListAsync(ct);
        db.CompanyOffices.RemoveRange(offices);
    }
}
