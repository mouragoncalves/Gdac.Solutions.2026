using MediatR;

namespace Gdac.Content.Application.Features.Products.Queries.GetProductPriceHistory;

public record GetProductPriceHistoryQuery(Guid ProductId) : IRequest<IReadOnlyList<ProductPriceHistoryResult>>;

public record ProductPriceHistoryResult(
    Guid    Id,
    decimal PrecoRevenda,
    decimal PrecoSugeridoFinal,
    decimal DescontoSugeridoSemestral,
    decimal DescontoSugeridoAnual,
    Guid    ChangedBy,
    DateTime ChangedAt,
    string? Notes);
