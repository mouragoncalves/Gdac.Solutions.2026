using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.BlockList.Queries.CheckCnpjBlocked;

public class CheckCnpjBlockedHandler(IBlockRecordRepository repo)
    : IRequestHandler<CheckCnpjBlockedQuery, bool>
{
    public Task<bool> Handle(CheckCnpjBlockedQuery request, CancellationToken ct)
        => repo.IsBlockedAsync(request.CnpjBase, ct);
}
