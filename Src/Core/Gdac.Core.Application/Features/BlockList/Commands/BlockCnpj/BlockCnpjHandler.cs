using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.BlockList.Commands.BlockCnpj;

public class BlockCnpjHandler(IBlockRecordRepository repo, IUnitOfWork uow)
    : IRequestHandler<BlockCnpjCommand>
{
    public async Task Handle(BlockCnpjCommand request, CancellationToken ct)
    {
        if (await repo.IsBlockedAsync(request.CnpjBase, ct))
            throw new DomainException("Este CNPJ já está na lista de bloqueio.");

        var record = BlockRecord.Create(request.CnpjBase, request.BlockedBy, request.Reason);
        await repo.AddAsync(record, ct);
        await uow.CommitAsync(ct);
    }
}
