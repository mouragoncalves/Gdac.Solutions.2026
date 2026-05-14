namespace Gdac.Auth.Domain.Entities;

public class Session
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid ApplicationId { get; private set; }
    public string RefreshTokenHash { get; private set; } = default!;
    public string IpAddress { get; private set; } = default!;
    public string DeviceInfo { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime LastActivityAt { get; private set; }
    public DateTime AbsoluteExpiration { get; private set; }
    public bool IsRevoked { get; private set; }

    public User User { get; private set; } = default!;
    public Application Application { get; private set; } = default!;

    private Session() { }

    public static Session Create(
        Guid userId,
        Guid applicationId,
        string refreshTokenHash,
        string ipAddress,
        string deviceInfo)
    {
        var now = DateTime.UtcNow;
        return new Session
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ApplicationId = applicationId,
            RefreshTokenHash = refreshTokenHash,
            IpAddress = ipAddress,
            DeviceInfo = deviceInfo,
            CreatedAt = now,
            LastActivityAt = now,
            AbsoluteExpiration = now.AddHours(8),
            IsRevoked = false
        };
    }

    public void RotateRefreshToken(string newHash)
    {
        RefreshTokenHash = newHash;
        LastActivityAt = DateTime.UtcNow;
    }

    public void Revoke()
    {
        IsRevoked = true;
    }

    public bool IsExpiredAbsolutely() =>
        DateTime.UtcNow >= AbsoluteExpiration;

    public bool IsIdleTimedOut(TimeSpan idleTimeout) =>
        DateTime.UtcNow >= LastActivityAt.Add(idleTimeout);

    public bool IsActive(TimeSpan idleTimeout) =>
        !IsRevoked && !IsExpiredAbsolutely() && !IsIdleTimedOut(idleTimeout);
}
