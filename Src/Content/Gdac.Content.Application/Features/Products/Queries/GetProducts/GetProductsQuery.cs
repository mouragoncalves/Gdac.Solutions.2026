using Gdac.Content.Application.Features.Products.Queries.GetProduct;
using MediatR;

namespace Gdac.Content.Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery : IRequest<IReadOnlyList<ProductResult>>;
