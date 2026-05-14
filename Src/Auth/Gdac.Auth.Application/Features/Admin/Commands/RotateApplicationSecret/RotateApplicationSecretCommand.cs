using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.RotateApplicationSecret;

public record RotateApplicationSecretCommand(Guid ApplicationId) : IRequest<RotateApplicationSecretResult>;
public record RotateApplicationSecretResult(string ClientSecret);
