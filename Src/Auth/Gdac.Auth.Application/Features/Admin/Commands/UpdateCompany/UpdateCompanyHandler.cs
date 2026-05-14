using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.UpdateCompany;

public class UpdateCompanyHandler(
    ICompanyRepository companies,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateCompanyCommand>
{
    public async Task Handle(UpdateCompanyCommand cmd, CancellationToken ct)
    {
        var company = await companies.FindByIdAsync(cmd.CompanyId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Company), cmd.CompanyId);

        company.Update(cmd.Name);
        if (cmd.IsActive) company.Activate(); else company.Deactivate();

        companies.Update(company);
        await unitOfWork.CommitAsync(ct);
    }
}
