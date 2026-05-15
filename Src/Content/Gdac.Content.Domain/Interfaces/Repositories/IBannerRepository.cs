using Gdac.Content.Domain.Entities;

namespace Gdac.Content.Domain.Interfaces.Repositories;

public interface IBannerRepository
{
    Task<Banner?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Banner>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Banner>> GetActiveAsync(Guid? partnerId, CancellationToken ct = default);
    Task AddAsync(Banner banner, CancellationToken ct = default);
    void Update(Banner banner);
}
