using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Session.Queries.GetApps;

public class GetAppsHandler(IApplicationRepository applications)
    : IRequestHandler<GetAppsQuery, GetAppsResult>
{
    public async Task<GetAppsResult> Handle(GetAppsQuery query, CancellationToken ct)
    {
        var apps = await applications.FindByUserIdAsync(query.UserId, ct);
        var dtos = apps.Select(a => new AppDto(a.ClientId, a.Name)).ToList();
        return new GetAppsResult(dtos);
    }
}
