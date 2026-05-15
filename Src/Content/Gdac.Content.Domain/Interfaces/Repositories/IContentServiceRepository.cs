using Gdac.Content.Domain.Entities;

namespace Gdac.Content.Domain.Interfaces.Repositories;

public interface IContentServiceRepository
{
    Task<ContentService?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ContentService?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ContentService>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ContentService>> GetActiveAsync(CancellationToken ct = default);
    Task AddAsync(ContentService service, CancellationToken ct = default);
    void Update(ContentService service);
}
