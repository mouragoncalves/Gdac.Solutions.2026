using MediatR;

namespace Gdac.Auth.Application.Features.Auth.Commands.LogoutAll;

public record LogoutAllCommand(Guid UserId) : IRequest;
