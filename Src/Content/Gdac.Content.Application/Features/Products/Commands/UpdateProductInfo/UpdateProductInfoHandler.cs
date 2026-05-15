using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Products.Commands.UpdateProductInfo;

public class UpdateProductInfoHandler(IProductRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateProductInfoCommand>
{
    public async Task Handle(UpdateProductInfoCommand request, CancellationToken ct)
    {
        var product = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        product.UpdateInfo(request.Name, request.Category, request.Description,
            request.IsFeatured, request.DisplayOrder);

        repo.Update(product);
        await uow.CommitAsync(ct);
    }
}
