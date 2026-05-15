using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Showcase.Commands.DeleteShowcaseItem;

public class DeleteShowcaseItemHandler(IShowcaseItemRepository repo, IUnitOfWork uow)
    : IRequestHandler<DeleteShowcaseItemCommand>
{
    public async Task Handle(DeleteShowcaseItemCommand request, CancellationToken ct)
    {
        var item = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(ShowcaseItem), request.Id);

        item.SetActive(false);
        repo.Update(item);
        await uow.CommitAsync(ct);
    }
}
