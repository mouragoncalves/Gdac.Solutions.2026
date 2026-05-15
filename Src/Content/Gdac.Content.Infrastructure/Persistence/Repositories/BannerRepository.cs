using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Content.Infrastructure.Persistence.Repositories;

public class BannerRepository(ContentDbContext db) : IBannerRepository
{
    public Task<Banner?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Banners.FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<IReadOnlyList<Banner>> GetAllAsync(CancellationToken ct = default) =>
        await db.Banners.OrderBy(b => b.DisplayOrder).ToListAsync(ct);

    public async Task<IReadOnlyList<Banner>> GetActiveAsync(Guid? partnerId, CancellationToken ct = default) =>
        await db.Banners
            .Where(b => b.IsActive && (partnerId == null || b.PartnerId == partnerId))
            .OrderBy(b => b.DisplayOrder)
            .ToListAsync(ct);

    public async Task AddAsync(Banner banner, CancellationToken ct = default) =>
        await db.Banners.AddAsync(banner, ct);

    public void Update(Banner banner) =>
        db.Banners.Update(banner);
}
