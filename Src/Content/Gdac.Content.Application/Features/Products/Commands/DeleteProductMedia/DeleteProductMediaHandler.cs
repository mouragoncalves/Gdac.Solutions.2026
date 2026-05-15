using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Products.Commands.DeleteProductMedia;

public class DeleteProductMediaHandler(IProductRepository repo, IUnitOfWork uow)
    : IRequestHandler<DeleteProductMediaCommand>
{
    public async Task Handle(DeleteProductMediaCommand request, CancellationToken ct)
    {
        var product = await repo.GetByIdWithDetailsAsync(request.ProductId, ct)
            ?? throw new NotFoundException(nameof(Product), request.ProductId);

        var media = product.Media.FirstOrDefault(m => m.Id == request.MediaId)
            ?? throw new NotFoundException(nameof(ProductMedia), request.MediaId);

        product.Media.Remove(media);
        repo.Update(product);
        await uow.CommitAsync(ct);
    }
}
