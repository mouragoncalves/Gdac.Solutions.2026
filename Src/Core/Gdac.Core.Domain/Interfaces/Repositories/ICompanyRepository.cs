using Gdac.Core.Domain.Entities;

namespace Gdac.Core.Domain.Interfaces.Repositories;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Company?> GetByIdWithUsersAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Company>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken ct = default);
    Task AddAsync(Company company, CancellationToken ct = default);
    void Update(Company company);
}
