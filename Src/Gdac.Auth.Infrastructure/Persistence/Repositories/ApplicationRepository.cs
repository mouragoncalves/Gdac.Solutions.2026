using Gdac.Auth.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Auth.Infrastructure.Persistence.Repositories;

public class ApplicationRepository(AppDbContext context) : IApplicationRepository
{
    public Task<Domain.Entities.Application?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Applications.FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<Domain.Entities.Application?> FindByClientIdAsync(string clientId, CancellationToken ct = default) =>
        context.Applications.FirstOrDefaultAsync(a => a.ClientId == clientId, ct);

    public async Task<IReadOnlyList<Domain.Entities.Application>> FindByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.UserApplications
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.Application)
            .Where(a => a.IsActive)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Domain.Entities.Application application, CancellationToken ct = default) =>
        await context.Applications.AddAsync(application, ct);

    public void Update(Domain.Entities.Application application) =>
        context.Applications.Update(application);
}
