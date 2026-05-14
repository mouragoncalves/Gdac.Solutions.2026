using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.DeactivateUser;

public class DeactivateUserHandler(
    IUserRepository users,
    ISessionRepository sessions,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeactivateUserCommand>
{
    public async Task Handle(DeactivateUserCommand cmd, CancellationToken ct)
    {
        var user = await users.FindByIdAsync(cmd.UserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), cmd.UserId);

        user.Deactivate();
        users.Update(user);
        await sessions.RevokeAllByUserIdAsync(user.Id, ct);
        await unitOfWork.CommitAsync(ct);
    }
}
