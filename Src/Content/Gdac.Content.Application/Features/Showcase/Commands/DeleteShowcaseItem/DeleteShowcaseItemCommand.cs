using MediatR;

namespace Gdac.Content.Application.Features.Showcase.Commands.DeleteShowcaseItem;

public record DeleteShowcaseItemCommand(Guid Id) : IRequest;
