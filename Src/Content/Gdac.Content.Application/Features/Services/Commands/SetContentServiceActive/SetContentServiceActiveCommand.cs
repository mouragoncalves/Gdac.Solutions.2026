using MediatR;

namespace Gdac.Content.Application.Features.Services.Commands.SetContentServiceActive;

public record SetContentServiceActiveCommand(Guid Id, bool IsActive) : IRequest;
