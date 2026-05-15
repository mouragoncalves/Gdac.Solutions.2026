using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Products.Commands.AddProductMedia;

public class AddProductMediaHandler(IProductRepository repo, IUnitOfWork uow)
    : IRequestHandler<AddProductMediaCommand, Guid>
{
    public async Task<Guid> Handle(AddProductMediaCommand request, CancellationToken ct)
    {
        var product = await repo.GetByIdWithDetailsAsync(request.ProductId, ct)
            ?? throw new NotFoundException(nameof(Product), request.ProductId);

        var media = ProductMedia.Create(request.ProductId, request.Url, request.Type, request.DisplayOrder);
        product.Media.Add(media);

        repo.Update(product);
        await uow.CommitAsync(ct);
        return media.Id;
    }
}
