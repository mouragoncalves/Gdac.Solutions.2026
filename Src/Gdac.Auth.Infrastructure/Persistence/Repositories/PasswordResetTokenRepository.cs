using Gdac.Auth.Domain.Entities;
using Gdac.Auth.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gdac.Auth.Infrastructure.Persistence.Repositories;

public class PasswordResetTokenRepository(AppDbContext context) : IPasswordResetTokenRepository
{
    public Task<PasswordResetToken?> FindValidByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        context.PasswordResetTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow, ct);

    public Task<PasswordResetToken?> FindByTokenHashAsync(string tokenHash, CancellationToken ct = default) =>
        context.PasswordResetTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task AddAsync(PasswordResetToken token, CancellationToken ct = default) =>
        await context.PasswordResetTokens.AddAsync(token, ct);

    public async Task InvalidateAllByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        await context.PasswordResetTokens
            .Where(t => t.UserId == userId && !t.IsUsed)
            .ExecuteUpdateAsync(t => t.SetProperty(x => x.IsUsed, true), ct);
    }

    public void Update(PasswordResetToken token) =>
        context.PasswordResetTokens.Update(token);
}
