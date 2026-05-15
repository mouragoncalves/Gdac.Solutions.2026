using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Showcase.Commands.CreateShowcaseItem;

public class CreateShowcaseItemHandler(IShowcaseItemRepository repo, IUnitOfWork uow)
    : IRequestHandler<CreateShowcaseItemCommand, Guid>
{
    public async Task<Guid> Handle(CreateShowcaseItemCommand request, CancellationToken ct)
    {
        var item = ShowcaseItem.Create(request.Type, request.CoreCompanyId, request.Name, request.LogoUrl);
        await repo.AddAsync(item, ct);
        await uow.CommitAsync(ct);
        return item.Id;
    }
}
