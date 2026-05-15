using Gdac.Content.Domain.Enums;
using MediatR;

namespace Gdac.Content.Application.Features.Products.Queries.GetProduct;

public record GetProductQuery(Guid Id) : IRequest<ProductResult>;

public record ProductMediaResult(Guid Id, string Url, MediaType Type, int DisplayOrder);

public record ProductResult(
    Guid    Id,
    string  Name,
    string  Category,
    string  Description,
    bool    IsActive,
    bool    IsFeatured,
    int     DisplayOrder,
    decimal PrecoRevenda,
    decimal PrecoSugeridoFinal,
    decimal DescontoSugeridoSemestral,
    decimal DescontoSugeridoAnual,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<ProductMediaResult> Media);
