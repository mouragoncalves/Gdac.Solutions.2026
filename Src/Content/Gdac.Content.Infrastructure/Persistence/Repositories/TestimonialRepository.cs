using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Content.Infrastructure.Persistence.Repositories;

public class TestimonialRepository(ContentDbContext db) : ITestimonialRepository
{
    public Task<Testimonial?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Testimonials.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<Testimonial>> GetAllAsync(CancellationToken ct = default) =>
        await db.Testimonials.OrderBy(t => t.DisplayOrder).ToListAsync(ct);

    public async Task<IReadOnlyList<Testimonial>> GetActiveAsync(Guid? partnerId, CancellationToken ct = default) =>
        await db.Testimonials
            .Where(t => t.IsActive && (partnerId == null || t.PartnerId == partnerId))
            .OrderBy(t => t.DisplayOrder)
            .ToListAsync(ct);

    public async Task AddAsync(Testimonial testimonial, CancellationToken ct = default) =>
        await db.Testimonials.AddAsync(testimonial, ct);

    public void Update(Testimonial testimonial) =>
        db.Testimonials.Update(testimonial);
}
