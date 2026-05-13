using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Queries.GetApplicationById;

public record GetApplicationByIdQuery(Guid ApplicationId) : IRequest<GetApplicationByIdResult>;
public record GetApplicationByIdResult(Guid Id, string Name, string ClientId, bool IsActive, DateTime CreatedAt);
