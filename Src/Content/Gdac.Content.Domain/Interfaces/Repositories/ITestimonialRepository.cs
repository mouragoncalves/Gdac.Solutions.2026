using Gdac.Content.Domain.Entities;

namespace Gdac.Content.Domain.Interfaces.Repositories;

public interface ITestimonialRepository
{
    Task<Testimonial?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Testimonial>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Testimonial>> GetActiveAsync(Guid? partnerId, CancellationToken ct = default);
    Task AddAsync(Testimonial testimonial, CancellationToken ct = default);
    void Update(Testimonial testimonial);
}
