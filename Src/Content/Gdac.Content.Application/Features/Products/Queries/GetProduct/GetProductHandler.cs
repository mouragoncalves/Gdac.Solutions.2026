using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Products.Queries.GetProduct;

public class GetProductHandler(IProductRepository repo)
    : IRequestHandler<GetProductQuery, ProductResult>
{
    public async Task<ProductResult> Handle(GetProductQuery request, CancellationToken ct)
    {
        var p = await repo.GetByIdWithDetailsAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        return MapToResult(p);
    }

    internal static ProductResult MapToResult(Product p) => new(
        p.Id, p.Name, p.Category, p.Description,
        p.IsActive, p.IsFeatured, p.DisplayOrder,
        p.PrecoRevenda, p.PrecoSugeridoFinal,
        p.DescontoSugeridoSemestral, p.DescontoSugeridoAnual,
        p.CreatedAt, p.UpdatedAt,
        p.Media.OrderBy(m => m.DisplayOrder)
            .Select(m => new ProductMediaResult(m.Id, m.Url, m.Type, m.DisplayOrder))
            .ToList());
}
