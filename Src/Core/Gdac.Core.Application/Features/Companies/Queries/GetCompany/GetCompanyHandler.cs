using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Queries.GetCompany;

public class GetCompanyHandler(ICompanyRepository repo)
    : IRequestHandler<GetCompanyQuery, CompanyResult>
{
    public async Task<CompanyResult> Handle(GetCompanyQuery request, CancellationToken ct)
    {
        var c = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Empresa", request.Id);

        return new CompanyResult(
            c.Id, c.Name, c.TradeName, c.CnpjBase, c.Cnpj,
            c.Type, c.Status, c.Segment, c.SizeCategory,
            c.Email, c.Phone,
            c.NatureId, c.NatureText,
            c.SizeId, c.SizeAcronym, c.SizeText,
            c.Equity, c.Jurisdiction,
            c.SimplesOptant, c.SimplesSince,
            c.SimeiOptant, c.SimeiSince,
            c.CreatedAt, c.UpdatedAt);
    }
}
