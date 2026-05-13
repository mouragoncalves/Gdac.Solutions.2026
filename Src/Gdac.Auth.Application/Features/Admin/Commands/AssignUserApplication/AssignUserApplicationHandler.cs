using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.AssignUserApplication;

public class AssignUserApplicationHandler(
    IUserRepository users,
    IApplicationRepository applications,
    IUnitOfWork unitOfWork
) : IRequestHandler<AssignUserApplicationCommand>
{
    public async Task Handle(AssignUserApplicationCommand cmd, CancellationToken ct)
    {
        if (await users.FindByIdAsync(cmd.UserId, ct) is null)
            throw new NotFoundException(nameof(Domain.Entities.User), cmd.UserId);
        if (await applications.FindByIdAsync(cmd.ApplicationId, ct) is null)
            throw new NotFoundException(nameof(Domain.Entities.Application), cmd.ApplicationId);

        await users.AssignApplicationAsync(cmd.UserId, cmd.ApplicationId, ct);
        await unitOfWork.CommitAsync(ct);
    }
}
