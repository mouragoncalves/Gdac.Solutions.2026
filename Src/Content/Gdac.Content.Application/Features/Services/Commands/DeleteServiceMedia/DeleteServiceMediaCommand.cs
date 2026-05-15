using MediatR;

namespace Gdac.Content.Application.Features.Services.Commands.DeleteServiceMedia;

public record DeleteServiceMediaCommand(Guid ServiceId, Guid MediaId) : IRequest;
