using MediatR;

namespace Gdac.Auth.Application.Features.Demo.Commands.RegisterDemo;

public record RegisterDemoCommand(string Name, string Email, string ClientId, string ClientSecret) : IRequest<RegisterDemoResult>;

public record RegisterDemoResult(string Message);
