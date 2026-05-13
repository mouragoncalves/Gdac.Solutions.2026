using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Queries.GetCompanyById;

public class GetCompanyByIdHandler(ICompanyRepository companies)
    : IRequestHandler<GetCompanyByIdQuery, GetCompanyByIdResult>
{
    public async Task<GetCompanyByIdResult> Handle(GetCompanyByIdQuery query, CancellationToken ct)
    {
        var company = await companies.FindByIdAsync(query.CompanyId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Company), query.CompanyId);
        return new GetCompanyByIdResult(company.Id, company.ExternalId, company.Name, company.IsActive);
    }
}
