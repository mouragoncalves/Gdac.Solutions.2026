using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Enums;

namespace Gdac.Content.Domain.Interfaces.Repositories;

public interface IShowcaseItemRepository
{
    Task<ShowcaseItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ShowcaseItem>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ShowcaseItem>> GetActiveAsync(ShowcaseItemType? type, CancellationToken ct = default);
    Task AddAsync(ShowcaseItem item, CancellationToken ct = default);
    void Update(ShowcaseItem item);
}
