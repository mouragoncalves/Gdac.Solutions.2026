using Gdac.Auth.Domain.Entities;
using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.CreateCompany;

public class CreateCompanyHandler(
    ICompanyRepository companies,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateCompanyCommand, CreateCompanyResult>
{
    public async Task<CreateCompanyResult> Handle(CreateCompanyCommand cmd, CancellationToken ct)
    {
        var company = Company.Create(cmd.ExternalId, cmd.Name);
        await companies.AddAsync(company, ct);
        await unitOfWork.CommitAsync(ct);
        return new CreateCompanyResult(company.Id, company.ExternalId, company.Name);
    }
}
