using MediatR;

namespace Gdac.Core.Application.Features.Companies.Queries.GetCompanyUsers;

public record GetCompanyUsersQuery(Guid CompanyId) : IRequest<IReadOnlyList<CompanyUserResult>>;

public record CompanyUserResult(Guid UserId, string FullName, string Email, string Role, bool IsActive, DateTime JoinedAt);
