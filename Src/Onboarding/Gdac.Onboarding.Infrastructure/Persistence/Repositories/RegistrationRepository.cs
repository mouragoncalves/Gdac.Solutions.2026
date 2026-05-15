using Gdac.Onboarding.Domain.Entities;
using Gdac.Onboarding.Domain.Enums;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Onboarding.Infrastructure.Persistence.Repositories;

public class RegistrationRepository(OnboardingDbContext db) : IRegistrationRepository
{
    public Task<Registration?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Registrations.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<Registration>> GetAllAsync(CancellationToken ct = default)
        => await db.Registrations.OrderByDescending(r => r.SubmittedAt).ToListAsync(ct);

    public async Task<IReadOnlyList<Registration>> GetByStatusAsync(RegistrationStatus status, CancellationToken ct = default)
        => await db.Registrations
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync(ct);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => db.Registrations.AnyAsync(r => r.ContactEmail == email.ToLowerInvariant(), ct);

    public Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken ct = default)
        => db.Registrations.AnyAsync(r => r.Cnpj == cnpj.Trim(), ct);

    public async Task AddAsync(Registration registration, CancellationToken ct = default)
        => await db.Registrations.AddAsync(registration, ct);

    public void Update(Registration registration)
        => db.Registrations.Update(registration);
}
