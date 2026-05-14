using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.RemoveUserApplication;

public class RemoveUserApplicationHandler(
    IUserRepository users,
    IUnitOfWork unitOfWork
) : IRequestHandler<RemoveUserApplicationCommand>
{
    public async Task Handle(RemoveUserApplicationCommand cmd, CancellationToken ct)
    {
        await users.RemoveApplicationAsync(cmd.UserId, cmd.ApplicationId, ct);
        await unitOfWork.CommitAsync(ct);
    }
}
