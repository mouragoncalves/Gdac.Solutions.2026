using Gdac.Core.Domain.Enums;
using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.DeactivateCompany;

public class DeactivateCompanyHandler(ICompanyRepository repo, IUnitOfWork uow)
    : IRequestHandler<DeactivateCompanyCommand>
{
    public async Task Handle(DeactivateCompanyCommand request, CancellationToken ct)
    {
        var company = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Empresa", request.Id);

        company.SetStatus(CompanyStatus.Inactive);
        repo.Update(company);
        await uow.CommitAsync(ct);
    }
}
