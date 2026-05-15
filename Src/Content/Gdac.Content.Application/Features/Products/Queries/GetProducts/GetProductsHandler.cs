using Gdac.Content.Application.Features.Products.Queries.GetProduct;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Products.Queries.GetProducts;

public class GetProductsHandler(IProductRepository repo)
    : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductResult>>
{
    public async Task<IReadOnlyList<ProductResult>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var list = await repo.GetActiveAsync(ct);
        return list.Select(GetProductHandler.MapToResult).ToList();
    }
}
