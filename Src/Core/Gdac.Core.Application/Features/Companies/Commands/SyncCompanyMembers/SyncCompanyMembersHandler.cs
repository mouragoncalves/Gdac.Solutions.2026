using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Core.Application.Features.Companies.Commands.SyncCompanyMembers;

public class SyncCompanyMembersHandler(
    ICompanyRepository companyRepo,
    ICompanyMemberRepository memberRepo,
    IUnitOfWork uow)
    : IRequestHandler<SyncCompanyMembersCommand>
{
    public async Task Handle(SyncCompanyMembersCommand request, CancellationToken ct)
    {
        if (!await companyRepo.ExistsAsync(request.CompanyId, ct))
            throw new NotFoundException("Empresa", request.CompanyId);

        await memberRepo.RemoveAllByCompanyIdAsync(request.CompanyId, ct);

        var members = request.Members.Select(m => CompanyMember.Create(
            request.CompanyId, m.PersonType, m.PersonName, m.RoleId, m.RoleText,
            m.Since, m.PersonExternalId, m.PersonTaxId, m.PersonAge,
            m.AgentName, m.AgentTaxId, m.AgentRoleId, m.AgentRoleText));

        await memberRepo.AddRangeAsync(members, ct);
        await uow.CommitAsync(ct);
    }
}
