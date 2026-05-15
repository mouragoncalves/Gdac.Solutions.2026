using Gdac.Onboarding.Domain.Entities;
using Gdac.Onboarding.Domain.Enums;

namespace Gdac.Onboarding.Domain.Interfaces.Repositories;

public interface IRegistrationRepository
{
    Task<Registration?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Registration>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Registration>> GetByStatusAsync(RegistrationStatus status, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken ct = default);
    Task AddAsync(Registration registration, CancellationToken ct = default);
    void Update(Registration registration);
}
