using Gdac.Core.Domain.Enums;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Queries.GetCompanyMembers;

public record GetCompanyMembersQuery(Guid CompanyId) : IRequest<IReadOnlyList<CompanyMemberResult>>;

public record CompanyMemberResult(
    Guid Id,
    DateOnly? Since,
    PersonType PersonType,
    string PersonName,
    string? PersonTaxId,
    string? PersonAge,
    int RoleId,
    string RoleText,
    string? AgentName,
    string? AgentTaxId,
    int? AgentRoleId,
    string? AgentRoleText);
