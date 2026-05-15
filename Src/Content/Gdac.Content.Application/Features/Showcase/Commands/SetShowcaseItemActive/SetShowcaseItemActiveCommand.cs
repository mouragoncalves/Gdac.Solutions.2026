using MediatR;

namespace Gdac.Content.Application.Features.Showcase.Commands.SetShowcaseItemActive;

public record SetShowcaseItemActiveCommand(Guid Id, bool IsActive) : IRequest;
