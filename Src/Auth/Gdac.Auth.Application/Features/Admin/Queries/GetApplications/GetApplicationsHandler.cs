using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Queries.GetApplications;

public class GetApplicationsHandler(IApplicationRepository applications)
    : IRequestHandler<GetApplicationsQuery, GetApplicationsResult>
{
    public async Task<GetApplicationsResult> Handle(GetApplicationsQuery query, CancellationToken ct)
    {
        var apps = await applications.GetAllAsync(ct);
        var dtos = apps.Select(a => new ApplicationSummaryDto(a.Id, a.Name, a.ClientId, a.IsActive, a.CreatedAt)).ToList();
        return new GetApplicationsResult(dtos);
    }
}
