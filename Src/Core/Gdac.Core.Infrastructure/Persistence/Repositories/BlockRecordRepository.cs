using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Core.Infrastructure.Persistence.Repositories;

public class BlockRecordRepository(CoreDbContext db) : IBlockRecordRepository
{
    public Task<bool> IsBlockedAsync(string cnpjBase, CancellationToken ct = default)
        => db.BlockRecords.AnyAsync(r => r.CnpjBase == cnpjBase, ct);

    public Task<BlockRecord?> GetByCnpjBaseAsync(string cnpjBase, CancellationToken ct = default)
        => db.BlockRecords.FirstOrDefaultAsync(r => r.CnpjBase == cnpjBase, ct);

    public async Task<IReadOnlyList<BlockRecord>> GetAllAsync(CancellationToken ct = default)
        => await db.BlockRecords.OrderByDescending(r => r.CreatedAt).ToListAsync(ct);

    public async Task AddAsync(BlockRecord record, CancellationToken ct = default)
        => await db.BlockRecords.AddAsync(record, ct);

    public void Remove(BlockRecord record)
        => db.BlockRecords.Remove(record);
}
