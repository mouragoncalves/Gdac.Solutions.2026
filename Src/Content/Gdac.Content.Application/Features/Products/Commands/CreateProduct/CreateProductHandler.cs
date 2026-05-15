using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Products.Commands.CreateProduct;

public class CreateProductHandler(IProductRepository repo, IUnitOfWork uow)
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = Product.Create(
            request.Name, request.Category, request.Description,
            request.PrecoRevenda, request.PrecoSugeridoFinal,
            request.DescontoSemestral, request.DescontoAnual);

        await repo.AddAsync(product, ct);
        await uow.CommitAsync(ct);
        return product.Id;
    }
}
