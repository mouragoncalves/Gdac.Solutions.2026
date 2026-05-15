using MediatR;

namespace Gdac.Content.Application.Features.Products.Commands.UpdateProductInfo;

public record UpdateProductInfoCommand(
    Guid   Id,
    string Name,
    string Category,
    string Description,
    bool   IsFeatured,
    int    DisplayOrder) : IRequest;
