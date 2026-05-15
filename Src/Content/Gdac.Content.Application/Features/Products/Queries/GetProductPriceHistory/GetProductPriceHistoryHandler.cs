using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Products.Queries.GetProductPriceHistory;

public class GetProductPriceHistoryHandler(IProductRepository repo)
    : IRequestHandler<GetProductPriceHistoryQuery, IReadOnlyList<ProductPriceHistoryResult>>
{
    public async Task<IReadOnlyList<ProductPriceHistoryResult>> Handle(
        GetProductPriceHistoryQuery request, CancellationToken ct)
    {
        var p = await repo.GetByIdWithDetailsAsync(request.ProductId, ct)
            ?? throw new NotFoundException(nameof(Product), request.ProductId);

        return p.PriceHistory
            .OrderByDescending(h => h.ChangedAt)
            .Select(h => new ProductPriceHistoryResult(
                h.Id, h.PrecoRevenda, h.PrecoSugeridoFinal,
                h.DescontoSugeridoSemestral, h.DescontoSugeridoAnual,
                h.ChangedBy, h.ChangedAt, h.Notes))
            .ToList();
    }
}
