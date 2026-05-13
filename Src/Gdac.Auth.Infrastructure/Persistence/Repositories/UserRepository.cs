using Gdac.Auth.Domain.Entities;
using Gdac.Auth.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Auth.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<User?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> FindByEmailAsync(string email, CancellationToken ct = default) =>
        context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant().Trim(), ct);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) =>
        context.Users.AnyAsync(u => u.Email == email.ToLowerInvariant().Trim(), ct);

    public Task<bool> HasAccessToApplicationAsync(Guid userId, Guid applicationId, CancellationToken ct = default) =>
        context.UserApplications.AnyAsync(ua => ua.UserId == userId && ua.ApplicationId == applicationId, ct);

    public async Task<(IReadOnlyList<User> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.Users.OrderBy(u => u.Email);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await context.Users.AddAsync(user, ct);

    public void Update(User user) =>
        context.Users.Update(user);

    public async Task AssignApplicationAsync(Guid userId, Guid applicationId, CancellationToken ct = default)
    {
        var exists = await context.UserApplications
            .AnyAsync(ua => ua.UserId == userId && ua.ApplicationId == applicationId, ct);
        if (!exists)
            await context.UserApplications.AddAsync(UserApplication.Create(userId, applicationId), ct);
    }

    public async Task RemoveApplicationAsync(Guid userId, Guid applicationId, CancellationToken ct = default)
    {
        await context.UserApplications
            .Where(ua => ua.UserId == userId && ua.ApplicationId == applicationId)
            .ExecuteDeleteAsync(ct);
    }

    public async Task AssignCompanyAsync(Guid userId, Guid companyId, CancellationToken ct = default)
    {
        var exists = await context.UserCompanies
            .AnyAsync(uc => uc.UserId == userId && uc.CompanyId == companyId, ct);
        if (!exists)
            await context.UserCompanies.AddAsync(UserCompany.Create(userId, companyId), ct);
    }

    public async Task RemoveCompanyAsync(Guid userId, Guid companyId, CancellationToken ct = default)
    {
        await context.UserCompanies
            .Where(uc => uc.UserId == userId && uc.CompanyId == companyId)
            .ExecuteDeleteAsync(ct);
    }
}
