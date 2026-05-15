using MediatR;

namespace Gdac.Content.Application.Features.Services.Commands.CreateContentService;

public record CreateContentServiceCommand(
    string  Name,
    string  Category,
    string  Description,
    decimal PrecoRevenda,
    decimal PrecoSugeridoFinal,
    decimal DescontoSemestral = 10m,
    decimal DescontoAnual     = 25m) : IRequest<Guid>;
