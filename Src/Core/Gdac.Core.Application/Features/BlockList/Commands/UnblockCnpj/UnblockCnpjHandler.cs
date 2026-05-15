using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.BlockList.Commands.UnblockCnpj;

public class UnblockCnpjHandler(IBlockRecordRepository repo, IUnitOfWork uow)
    : IRequestHandler<UnblockCnpjCommand>
{
    public async Task Handle(UnblockCnpjCommand request, CancellationToken ct)
    {
        var record = await repo.GetByCnpjBaseAsync(request.CnpjBase, ct)
            ?? throw new NotFoundException("BlockRecord", request.CnpjBase);

        repo.Remove(record);
        await uow.CommitAsync(ct);
    }
}
