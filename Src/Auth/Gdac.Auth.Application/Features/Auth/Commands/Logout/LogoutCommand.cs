using MediatR;

namespace Gdac.Auth.Application.Features.Auth.Commands.Logout;

public record LogoutCommand(Guid SessionId) : IRequest;
