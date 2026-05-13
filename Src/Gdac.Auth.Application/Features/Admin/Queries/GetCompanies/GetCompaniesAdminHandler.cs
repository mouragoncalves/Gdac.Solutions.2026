using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Queries.GetCompanies;

public class GetCompaniesAdminHandler(ICompanyRepository companies)
    : IRequestHandler<GetCompaniesAdminQuery, GetCompaniesAdminResult>
{
    public async Task<GetCompaniesAdminResult> Handle(GetCompaniesAdminQuery query, CancellationToken ct)
    {
        var list = await companies.GetAllAsync(ct);
        var dtos = list.Select(c => new CompanySummaryDto(c.Id, c.ExternalId, c.Name, c.IsActive)).ToList();
        return new GetCompaniesAdminResult(dtos);
    }
}
