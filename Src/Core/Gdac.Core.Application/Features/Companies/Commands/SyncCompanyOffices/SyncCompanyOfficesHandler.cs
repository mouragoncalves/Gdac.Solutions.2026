using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.SyncCompanyOffices;

public class SyncCompanyOfficesHandler(
    ICompanyRepository companyRepo,
    ICompanyOfficeRepository officeRepo,
    IUnitOfWork uow)
    : IRequestHandler<SyncCompanyOfficesCommand>
{
    public async Task Handle(SyncCompanyOfficesCommand request, CancellationToken ct)
    {
        if (!await companyRepo.ExistsAsync(request.CompanyId, ct))
            throw new NotFoundException("Empresa", request.CompanyId);

        await officeRepo.RemoveAllByCompanyIdAsync(request.CompanyId, ct);

        var offices = request.Offices.Select(o => CompanyOffice.Create(
            request.CompanyId, o.TaxId, o.StatusId, o.StatusText,
            o.IsHead, o.Alias, o.Founded, o.StatusDate,
            o.ReasonId, o.ReasonText, o.MainActivityId, o.MainActivityText));

        await officeRepo.AddRangeAsync(offices, ct);
        await uow.CommitAsync(ct);
    }
}
