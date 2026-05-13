namespace Gdac.Auth.Domain.Entities;

public class PasswordResetToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public bool IsUsed { get; private set; }

    public User User { get; private set; } = default!;

    private PasswordResetToken() { }

    public static PasswordResetToken Create(Guid userId, string tokenHash)
    {
        return new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            IsUsed = false
        };
    }

    public void MarkAsUsed()
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
    }

    public bool IsValid() =>
        !IsUsed && DateTime.UtcNow < ExpiresAt;
}
