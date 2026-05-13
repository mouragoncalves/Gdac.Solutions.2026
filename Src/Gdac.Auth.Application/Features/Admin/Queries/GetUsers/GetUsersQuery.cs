using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Queries.GetUsers;

public record GetUsersQuery(int Page = 1, int PageSize = 20) : IRequest<GetUsersResult>;

public record UserSummaryDto(Guid Id, string Email, bool IsActive, bool MustChangePassword, DateTime CreatedAt);
public record GetUsersResult(IReadOnlyList<UserSummaryDto> Items, int Total, int Page, int PageSize);
