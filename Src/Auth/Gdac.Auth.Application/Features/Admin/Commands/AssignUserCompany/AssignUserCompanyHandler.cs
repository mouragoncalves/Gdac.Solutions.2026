using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.AssignUserCompany;

public class AssignUserCompanyHandler(
    IUserRepository users,
    ICompanyRepository companies,
    IUnitOfWork unitOfWork
) : IRequestHandler<AssignUserCompanyCommand>
{
    public async Task Handle(AssignUserCompanyCommand cmd, CancellationToken ct)
    {
        if (await users.FindByIdAsync(cmd.UserId, ct) is null)
            throw new NotFoundException(nameof(Domain.Entities.User), cmd.UserId);
        if (await companies.FindByIdAsync(cmd.CompanyId, ct) is null)
            throw new NotFoundException(nameof(Domain.Entities.Company), cmd.CompanyId);

        await users.AssignCompanyAsync(cmd.UserId, cmd.CompanyId, ct);
        await unitOfWork.CommitAsync(ct);
    }
}
