using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Showcase.Commands.UpdateShowcaseItem;

public class UpdateShowcaseItemHandler(IShowcaseItemRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateShowcaseItemCommand>
{
    public async Task Handle(UpdateShowcaseItemCommand request, CancellationToken ct)
    {
        var item = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(ShowcaseItem), request.Id);

        item.Update(request.Type, request.CoreCompanyId, request.Name, request.LogoUrl);
        repo.Update(item);
        await uow.CommitAsync(ct);
    }
}
