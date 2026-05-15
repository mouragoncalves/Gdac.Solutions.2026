using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Enums;

namespace Gdac.Content.Domain.Interfaces.Repositories;

public interface IIntegrationRepository
{
    Task<Integration?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Integration>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Integration>> GetActiveAsync(IntegrationCategory? category, CancellationToken ct = default);
    Task AddAsync(Integration integration, CancellationToken ct = default);
    void Update(Integration integration);
}
