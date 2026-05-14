using Gdac.Auth.Domain.Entities;

namespace Gdac.Auth.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> HasAccessToApplicationAsync(Guid userId, Guid applicationId, CancellationToken ct = default);
    Task<(IReadOnlyList<User> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    void Update(User user);
    Task AssignApplicationAsync(Guid userId, Guid applicationId, CancellationToken ct = default);
    Task RemoveApplicationAsync(Guid userId, Guid applicationId, CancellationToken ct = default);
    Task AssignCompanyAsync(Guid userId, Guid companyId, CancellationToken ct = default);
    Task RemoveCompanyAsync(Guid userId, Guid companyId, CancellationToken ct = default);
}
