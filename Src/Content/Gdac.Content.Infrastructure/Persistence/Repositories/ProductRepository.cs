using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Content.Infrastructure.Persistence.Repositories;

public class ProductRepository(ContentDbContext db) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Product?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default) =>
        db.Products
            .Include(p => p.Media)
            .Include(p => p.PriceHistory)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default) =>
        await db.Products.Include(p => p.Media).OrderBy(p => p.DisplayOrder).ToListAsync(ct);

    public async Task<IReadOnlyList<Product>> GetActiveAsync(CancellationToken ct = default) =>
        await db.Products
            .Include(p => p.Media)
            .Where(p => p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(ct);

    public async Task AddAsync(Product product, CancellationToken ct = default) =>
        await db.Products.AddAsync(product, ct);

    public void Update(Product product) =>
        db.Products.Update(product);
}
