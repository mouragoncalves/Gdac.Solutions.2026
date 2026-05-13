using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.CreateApplication;

public record CreateApplicationCommand(string Name, string ClientId) : IRequest<CreateApplicationResult>;
public record CreateApplicationResult(Guid Id, string Name, string ClientId, string ClientSecret);
