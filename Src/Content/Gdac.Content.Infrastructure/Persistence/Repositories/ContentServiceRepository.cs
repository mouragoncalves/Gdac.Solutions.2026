using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Content.Infrastructure.Persistence.Repositories;

public class ContentServiceRepository(ContentDbContext db) : IContentServiceRepository
{
    public Task<ContentService?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Services.FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<ContentService?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default) =>
        db.Services
            .Include(s => s.Media)
            .Include(s => s.PriceHistory)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<ContentService>> GetAllAsync(CancellationToken ct = default) =>
        await db.Services.Include(s => s.Media).OrderBy(s => s.DisplayOrder).ToListAsync(ct);

    public async Task<IReadOnlyList<ContentService>> GetActiveAsync(CancellationToken ct = default) =>
        await db.Services
            .Include(s => s.Media)
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(ct);

    public async Task AddAsync(ContentService service, CancellationToken ct = default) =>
        await db.Services.AddAsync(service, ct);

    public void Update(ContentService service) =>
        db.Services.Update(service);
}
