using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.RemoveUserCompany;

public class RemoveUserCompanyHandler(
    IUserRepository users,
    IUnitOfWork unitOfWork
) : IRequestHandler<RemoveUserCompanyCommand>
{
    public async Task Handle(RemoveUserCompanyCommand cmd, CancellationToken ct)
    {
        await users.RemoveCompanyAsync(cmd.UserId, cmd.CompanyId, ct);
        await unitOfWork.CommitAsync(ct);
    }
}
