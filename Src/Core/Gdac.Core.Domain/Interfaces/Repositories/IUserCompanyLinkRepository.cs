using Gdac.Core.Domain.Entities;

namespace Gdac.Core.Domain.Interfaces.Repositories;

public interface IUserCompanyLinkRepository
{
    Task<UserCompanyLink?> GetAsync(Guid userId, Guid companyId, CancellationToken ct = default);
    Task<IReadOnlyList<UserCompanyLink>> GetByCompanyAsync(Guid companyId, CancellationToken ct = default);
    Task<IReadOnlyList<UserCompanyLink>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(UserCompanyLink link, CancellationToken ct = default);
    void Update(UserCompanyLink link);
}
