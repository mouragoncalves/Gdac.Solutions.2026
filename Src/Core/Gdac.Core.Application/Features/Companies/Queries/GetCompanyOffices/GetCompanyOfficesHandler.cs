using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Queries.GetCompanyOffices;

public class GetCompanyOfficesHandler(ICompanyRepository companyRepo, ICompanyOfficeRepository officeRepo)
    : IRequestHandler<GetCompanyOfficesQuery, IReadOnlyList<CompanyOfficeResult>>
{
    public async Task<IReadOnlyList<CompanyOfficeResult>> Handle(GetCompanyOfficesQuery request, CancellationToken ct)
    {
        if (!await companyRepo.ExistsAsync(request.CompanyId, ct))
            throw new NotFoundException("Empresa", request.CompanyId);

        var offices = await officeRepo.GetByCompanyIdAsync(request.CompanyId, ct);

        return offices.Select(o => new CompanyOfficeResult(
            o.Id, o.TaxId, o.Alias, o.Founded, o.IsHead,
            o.StatusId, o.StatusText, o.StatusDate,
            o.ReasonId, o.ReasonText,
            o.MainActivityId, o.MainActivityText)).ToList();
    }
}
