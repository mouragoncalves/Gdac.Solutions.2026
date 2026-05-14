using Gdac.Core.Domain.Enums;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.SyncCompanyMembers;

public record SyncCompanyMembersCommand(
    Guid CompanyId,
    IReadOnlyList<MemberInput> Members) : IRequest;

public record MemberInput(
    PersonType PersonType,
    string PersonName,
    int RoleId,
    string RoleText,
    DateOnly? Since = null,
    string? PersonExternalId = null,
    string? PersonTaxId = null,
    string? PersonAge = null,
    string? AgentName = null,
    string? AgentTaxId = null,
    int? AgentRoleId = null,
    string? AgentRoleText = null);
