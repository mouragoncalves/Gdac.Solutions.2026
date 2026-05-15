using Gdac.Content.Domain.Enums;
using MediatR;

namespace Gdac.Content.Application.Features.Products.Commands.AddProductMedia;

public record AddProductMediaCommand(
    Guid      ProductId,
    string    Url,
    MediaType Type,
    int       DisplayOrder) : IRequest<Guid>;
