using Gdac.Core.Application.Features.Companies.Queries.GetCompany;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Queries.GetCompanies;

public class GetCompaniesHandler(ICompanyRepository repo)
    : IRequestHandler<GetCompaniesQuery, IReadOnlyList<CompanyResult>>
{
    public async Task<IReadOnlyList<CompanyResult>> Handle(GetCompaniesQuery request, CancellationToken ct)
    {
        var list = await repo.GetAllAsync(ct);
        return list.Select(c => new CompanyResult(
            c.Id, c.Name, c.TradeName, c.CnpjBase, c.Cnpj,
            c.Type, c.Status, c.Segment, c.SizeCategory,
            c.Email, c.Phone,
            c.NatureId, c.NatureText,
            c.SizeId, c.SizeAcronym, c.SizeText,
            c.Equity, c.Jurisdiction,
            c.SimplesOptant, c.SimplesSince,
            c.SimeiOptant, c.SimeiSince,
            c.CreatedAt, c.UpdatedAt)).ToList();
    }
}
