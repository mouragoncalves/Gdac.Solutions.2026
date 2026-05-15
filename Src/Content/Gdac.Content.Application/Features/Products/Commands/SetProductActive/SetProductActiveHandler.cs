using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Products.Commands.SetProductActive;

public class SetProductActiveHandler(IProductRepository repo, IUnitOfWork uow)
    : IRequestHandler<SetProductActiveCommand>
{
    public async Task Handle(SetProductActiveCommand request, CancellationToken ct)
    {
        var product = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        product.SetActive(request.IsActive);
        repo.Update(product);
        await uow.CommitAsync(ct);
    }
}
