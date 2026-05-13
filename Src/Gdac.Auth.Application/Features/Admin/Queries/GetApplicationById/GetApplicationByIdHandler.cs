using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Queries.GetApplicationById;

public class GetApplicationByIdHandler(IApplicationRepository applications)
    : IRequestHandler<GetApplicationByIdQuery, GetApplicationByIdResult>
{
    public async Task<GetApplicationByIdResult> Handle(GetApplicationByIdQuery query, CancellationToken ct)
    {
        var app = await applications.FindByIdAsync(query.ApplicationId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Application), query.ApplicationId);
        return new GetApplicationByIdResult(app.Id, app.Name, app.ClientId, app.IsActive, app.CreatedAt);
    }
}
