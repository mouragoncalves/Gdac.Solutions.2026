using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.BlockList.Queries.GetBlockList;

public class GetBlockListHandler(IBlockRecordRepository repo)
    : IRequestHandler<GetBlockListQuery, IReadOnlyList<BlockRecordResult>>
{
    public async Task<IReadOnlyList<BlockRecordResult>> Handle(GetBlockListQuery request, CancellationToken ct)
    {
        var list = await repo.GetAllAsync(ct);
        return list.Select(r => new BlockRecordResult(r.Id, r.CnpjBase, r.Reason, r.BlockedBy, r.CreatedAt))
                   .ToList();
    }
}
