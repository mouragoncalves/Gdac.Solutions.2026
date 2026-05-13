using Gdac.Auth.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Queries.GetUsers;

public class GetUsersHandler(IUserRepository users) : IRequestHandler<GetUsersQuery, GetUsersResult>
{
    public async Task<GetUsersResult> Handle(GetUsersQuery query, CancellationToken ct)
    {
        var (items, total) = await users.GetPagedAsync(query.Page, query.PageSize, ct);
        var dtos = items.Select(u => new UserSummaryDto(
            u.Id, u.Email, u.IsActive, u.MustChangePassword, u.CreatedAt)).ToList();
        return new GetUsersResult(dtos, total, query.Page, query.PageSize);
    }
}
