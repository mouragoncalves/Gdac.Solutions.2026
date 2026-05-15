using MediatR;

namespace Gdac.Content.Application.Features.Services.Commands.UpdateContentServicePricing;

public record UpdateContentServicePricingCommand(
    Guid    Id,
    decimal PrecoRevenda,
    decimal PrecoSugeridoFinal,
    decimal DescontoSemestral,
    decimal DescontoAnual,
    Guid    ChangedBy,
    string? Notes) : IRequest;
