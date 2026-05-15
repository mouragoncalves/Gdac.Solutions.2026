using MediatR;

namespace Gdac.Content.Application.Features.Services.Queries.GetContentServicePriceHistory;

public record GetContentServicePriceHistoryQuery(Guid ServiceId) : IRequest<IReadOnlyList<ContentServicePriceHistoryResult>>;

public record ContentServicePriceHistoryResult(
    Guid    Id,
    decimal PrecoRevenda,
    decimal PrecoSugeridoFinal,
    decimal DescontoSugeridoSemestral,
    decimal DescontoSugeridoAnual,
    Guid    ChangedBy,
    DateTime ChangedAt,
    string? Notes);
