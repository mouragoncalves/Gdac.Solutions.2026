using MediatR;

namespace Gdac.Auth.Application.Features.Session.Queries.GetApps;

public record GetAppsQuery(Guid UserId) : IRequest<GetAppsResult>;

public record AppDto(string ClientId, string Name);

public record GetAppsResult(IReadOnlyList<AppDto> Applications);
