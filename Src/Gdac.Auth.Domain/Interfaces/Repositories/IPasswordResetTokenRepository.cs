using Gdac.Auth.Domain.Entities;

namespace Gdac.Auth.Domain.Interfaces.Repositories;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> FindValidByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<PasswordResetToken?> FindByTokenHashAsync(string tokenHash, CancellationToken ct = default);
    Task AddAsync(PasswordResetToken token, CancellationToken ct = default);
    Task InvalidateAllByUserIdAsync(Guid userId, CancellationToken ct = default);
    void Update(PasswordResetToken token);
}
