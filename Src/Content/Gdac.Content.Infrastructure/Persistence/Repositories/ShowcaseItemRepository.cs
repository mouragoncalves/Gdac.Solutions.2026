using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Enums;
using Gdac.Content.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Content.Infrastructure.Persistence.Repositories;

public class ShowcaseItemRepository(ContentDbContext db) : IShowcaseItemRepository
{
    public Task<ShowcaseItem?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.ShowcaseItems.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<ShowcaseItem>> GetAllAsync(CancellationToken ct = default) =>
        await db.ShowcaseItems.OrderBy(s => s.DisplayOrder).ToListAsync(ct);

    public async Task<IReadOnlyList<ShowcaseItem>> GetActiveAsync(ShowcaseItemType? type, CancellationToken ct = default) =>
        await db.ShowcaseItems
            .Where(s => s.IsActive && (type == null || s.Type == type))
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(ct);

    public async Task AddAsync(ShowcaseItem item, CancellationToken ct = default) =>
        await db.ShowcaseItems.AddAsync(item, ct);

    public void Update(ShowcaseItem item) =>
        db.ShowcaseItems.Update(item);
}
