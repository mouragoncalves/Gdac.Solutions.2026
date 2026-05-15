using MediatR;

namespace Gdac.Content.Application.Features.Products.Commands.SetProductActive;

public record SetProductActiveCommand(Guid Id, bool IsActive) : IRequest;
