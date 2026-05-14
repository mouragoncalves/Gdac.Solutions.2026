using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.SyncCompanyCnpjData;

public class SyncCompanyCnpjDataHandler(ICompanyRepository repo, IUnitOfWork uow)
    : IRequestHandler<SyncCompanyCnpjDataCommand>
{
    public async Task Handle(SyncCompanyCnpjDataCommand request, CancellationToken ct)
    {
        var company = await repo.GetByIdAsync(request.CompanyId, ct)
            ?? throw new NotFoundException("Empresa", request.CompanyId);

        company.SetCnpjData(
            request.CnpjBase,
            request.NatureId, request.NatureText,
            request.SizeId, request.SizeAcronym, request.SizeText,
            request.Equity, request.Jurisdiction,
            request.SimplesOptant, request.SimplesSince,
            request.SimeiOptant, request.SimeiSince);

        repo.Update(company);
        await uow.CommitAsync(ct);
    }
}
