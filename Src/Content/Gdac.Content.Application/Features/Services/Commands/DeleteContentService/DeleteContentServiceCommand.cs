using MediatR;

namespace Gdac.Content.Application.Features.Services.Commands.DeleteContentService;

public record DeleteContentServiceCommand(Guid Id) : IRequest;
