using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.UpdateUser;

public class UpdateUserHandler(
    IUserRepository users,
    ISessionRepository sessions,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand cmd, CancellationToken ct)
    {
        var user = await users.FindByIdAsync(cmd.UserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), cmd.UserId);

        if (cmd.IsActive) user.Activate();
        else
        {
            user.Deactivate();
            await sessions.RevokeAllByUserIdAsync(user.Id, ct);
        }

        if (cmd.MustChangePassword) user.RequirePasswordChange();

        users.Update(user);
        await unitOfWork.CommitAsync(ct);
    }
}
