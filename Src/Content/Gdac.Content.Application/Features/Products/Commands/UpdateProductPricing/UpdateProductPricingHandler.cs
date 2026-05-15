using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Products.Commands.UpdateProductPricing;

public class UpdateProductPricingHandler(IProductRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateProductPricingCommand>
{
    public async Task Handle(UpdateProductPricingCommand request, CancellationToken ct)
    {
        var product = await repo.GetByIdWithDetailsAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        product.UpdatePricing(
            request.PrecoRevenda, request.PrecoSugeridoFinal,
            request.DescontoSemestral, request.DescontoAnual,
            request.ChangedBy, request.Notes);

        repo.Update(product);
        await uow.CommitAsync(ct);
    }
}
