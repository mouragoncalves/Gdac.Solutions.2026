using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Session.Queries.GetCompanies;

public class GetCompaniesHandler(ICompanyRepository companies)
    : IRequestHandler<GetCompaniesQuery, GetCompaniesResult>
{
    public async Task<GetCompaniesResult> Handle(GetCompaniesQuery query, CancellationToken ct)
    {
        var list = await companies.FindByUserIdAsync(query.UserId, ct);
        var dtos = list.Select(c => new CompanyDto(c.Id, c.ExternalId, c.Name)).ToList();
        return new GetCompaniesResult(dtos);
    }
}
