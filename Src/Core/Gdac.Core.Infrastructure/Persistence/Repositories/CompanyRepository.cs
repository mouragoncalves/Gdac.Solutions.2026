using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Core.Infrastructure.Persistence.Repositories;

public class CompanyRepository(CoreDbContext db) : ICompanyRepository
{
    public Task<Company?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Companies.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Company?> GetByIdWithUsersAsync(Guid id, CancellationToken ct = default) =>
        db.Companies.Include(c => c.UserLinks).ThenInclude(l => l.User)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Company>> GetAllAsync(CancellationToken ct = default) =>
        await db.Companies.OrderBy(c => c.Name).ToListAsync(ct);

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        db.Companies.AnyAsync(c => c.Id == id, ct);

    public Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken ct = default) =>
        db.Companies.AnyAsync(c => c.Cnpj == cnpj, ct);

    public async Task AddAsync(Company company, CancellationToken ct = default) =>
        await db.Companies.AddAsync(company, ct);

    public void Update(Company company) =>
        db.Companies.Update(company);
}
