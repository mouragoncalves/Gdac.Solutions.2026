using MediatR;

namespace Gdac.Content.Application.Features.Products.Commands.UpdateProductPricing;

public record UpdateProductPricingCommand(
    Guid    Id,
    decimal PrecoRevenda,
    decimal PrecoSugeridoFinal,
    decimal DescontoSemestral,
    decimal DescontoAnual,
    Guid    ChangedBy,
    string? Notes) : IRequest;
