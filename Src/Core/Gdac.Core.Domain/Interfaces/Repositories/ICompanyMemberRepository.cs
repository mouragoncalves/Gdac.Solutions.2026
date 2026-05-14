using Gdac.Core.Domain.Entities;

namespace Gdac.Core.Domain.Interfaces.Repositories;

public interface ICompanyMemberRepository
{
    Task<IReadOnlyList<CompanyMember>> GetByCompanyIdAsync(Guid companyId, CancellationToken ct = default);
    Task<CompanyMember?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(CompanyMember member, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<CompanyMember> members, CancellationToken ct = default);
    void Remove(CompanyMember member);
    Task RemoveAllByCompanyIdAsync(Guid companyId, CancellationToken ct = default);
}
