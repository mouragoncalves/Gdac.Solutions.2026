using Gdac.Auth.Domain.Entities;
using Gdac.Auth.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Auth.Infrastructure.Persistence.Repositories;

public class CompanyRepository(AppDbContext context) : ICompanyRepository
{
    public Task<Company?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Companies.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Company>> FindByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.UserCompanies
            .Where(uc => uc.UserId == userId)
            .Select(uc => uc.Company)
            .Where(c => c.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Company company, CancellationToken ct = default) =>
        await context.Companies.AddAsync(company, ct);

    public void Update(Company company) =>
        context.Companies.Update(company);
}
