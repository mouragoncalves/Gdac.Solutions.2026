using MediatR;

namespace Gdac.Auth.Application.Features.Demo.Commands.RegisterDemo;

public record RegisterDemoCommand(string Name, string Email) : IRequest<RegisterDemoResult>;

public record RegisterDemoResult(string Message);
