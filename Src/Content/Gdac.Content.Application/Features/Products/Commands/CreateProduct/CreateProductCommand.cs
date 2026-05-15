using MediatR;

namespace Gdac.Content.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string  Name,
    string  Category,
    string  Description,
    decimal PrecoRevenda,
    decimal PrecoSugeridoFinal,
    decimal DescontoSemestral = 10m,
    decimal DescontoAnual     = 25m) : IRequest<Guid>;
