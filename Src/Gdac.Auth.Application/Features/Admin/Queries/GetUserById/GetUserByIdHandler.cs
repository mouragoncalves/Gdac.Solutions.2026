using Gdac.Auth.Domain.Exceptions;
using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Queries.GetUserById;

public class GetUserByIdHandler(IUserRepository users) : IRequestHandler<GetUserByIdQuery, GetUserByIdResult>
{
    public async Task<GetUserByIdResult> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var user = await users.FindByIdAsync(query.UserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), query.UserId);

        return new GetUserByIdResult(user.Id, user.Email, user.IsActive, user.MustChangePassword,
            user.FailedLoginAttempts, user.LockoutUntil, user.CreatedAt, user.UpdatedAt);
    }
}
