using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Queries.GetCompanyMembers;

public class GetCompanyMembersHandler(ICompanyRepository companyRepo, ICompanyMemberRepository memberRepo)
    : IRequestHandler<GetCompanyMembersQuery, IReadOnlyList<CompanyMemberResult>>
{
    public async Task<IReadOnlyList<CompanyMemberResult>> Handle(GetCompanyMembersQuery request, CancellationToken ct)
    {
        if (!await companyRepo.ExistsAsync(request.CompanyId, ct))
            throw new NotFoundException("Empresa", request.CompanyId);

        var members = await memberRepo.GetByCompanyIdAsync(request.CompanyId, ct);

        return members.Select(m => new CompanyMemberResult(
            m.Id, m.Since, m.PersonType, m.PersonName, m.PersonTaxId,
            m.PersonAge, m.RoleId, m.RoleText,
            m.AgentName, m.AgentTaxId, m.AgentRoleId, m.AgentRoleText)).ToList();
    }
}
