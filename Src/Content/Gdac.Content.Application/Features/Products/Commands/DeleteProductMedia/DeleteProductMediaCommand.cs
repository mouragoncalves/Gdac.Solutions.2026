using MediatR;

namespace Gdac.Content.Application.Features.Products.Commands.DeleteProductMedia;

public record DeleteProductMediaCommand(Guid ProductId, Guid MediaId) : IRequest;
