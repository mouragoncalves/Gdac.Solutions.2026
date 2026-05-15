using Gdac.Content.Domain.Enums;
using MediatR;

namespace Gdac.Content.Application.Features.Services.Queries.GetContentService;

public record GetContentServiceQuery(Guid Id) : IRequest<ContentServiceResult>;

public record ServiceMediaResult(Guid Id, string Url, MediaType Type, int DisplayOrder);

public record ContentServiceResult(
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
    IReadOnlyList<ServiceMediaResult> Media);
