using Gdac.Auth.Domain.Entities;

namespace Gdac.Auth.Domain.Interfaces.Repositories;

public interface ICompanyRepository
{
    Task<Company?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Company>> FindByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Company company, CancellationToken ct = default);
    void Update(Company company);
}
