using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Enums;
using Gdac.Content.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Content.Infrastructure.Persistence.Repositories;

public class IntegrationRepository(ContentDbContext db) : IIntegrationRepository
{
    public Task<Integration?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Integrations.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<IReadOnlyList<Integration>> GetAllAsync(CancellationToken ct = default) =>
        await db.Integrations.OrderBy(i => i.DisplayOrder).ToListAsync(ct);

    public async Task<IReadOnlyList<Integration>> GetActiveAsync(IntegrationCategory? category, CancellationToken ct = default) =>
        await db.Integrations
            .Where(i => i.IsActive && (category == null || i.Category == category))
            .OrderBy(i => i.DisplayOrder)
            .ToListAsync(ct);

    public async Task AddAsync(Integration integration, CancellationToken ct = default) =>
        await db.Integrations.AddAsync(integration, ct);

    public void Update(Integration integration) =>
        db.Integrations.Update(integration);
}
