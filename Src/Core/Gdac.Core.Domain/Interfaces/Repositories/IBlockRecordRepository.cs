using Gdac.Core.Domain.Entities;

namespace Gdac.Core.Domain.Interfaces.Repositories;

public interface IBlockRecordRepository
{
    Task<bool> IsBlockedAsync(string cnpjBase, CancellationToken ct = default);
    Task<BlockRecord?> GetByCnpjBaseAsync(string cnpjBase, CancellationToken ct = default);
    Task<IReadOnlyList<BlockRecord>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(BlockRecord record, CancellationToken ct = default);
    void Remove(BlockRecord record);
}
