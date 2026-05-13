using Gdac.Auth.Domain.Enums;

namespace Gdac.Auth.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public PasswordAlgorithm PasswordAlgorithm { get; private set; }
    public bool IsActive { get; private set; }
    public bool MustChangePassword { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutUntil { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public ICollection<UserApplication> Applications { get; private set; } = [];
    public ICollection<UserCompany> Companies { get; private set; } = [];
    public ICollection<Session> Sessions { get; private set; } = [];

    private User() { }

    public static User Create(string email, string passwordHash, PasswordAlgorithm algorithm)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant().Trim(),
            PasswordHash = passwordHash,
            PasswordAlgorithm = algorithm,
            IsActive = true,
            MustChangePassword = false,
            FailedLoginAttempts = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static User CreateDemo(string email, string passwordHash, PasswordAlgorithm algorithm)
    {
        var user = Create(email, passwordHash, algorithm);
        user.MustChangePassword = true;
        return user;
    }

    public void UpdatePassword(string passwordHash, PasswordAlgorithm algorithm)
    {
        PasswordHash = passwordHash;
        PasswordAlgorithm = algorithm;
        MustChangePassword = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RegisterFailedLogin()
    {
        FailedLoginAttempts++;
        LockoutUntil = FailedLoginAttempts switch
        {
            >= 10 => DateTime.UtcNow.AddHours(1),
            >= 5  => DateTime.UtcNow.AddMinutes(15),
            _     => LockoutUntil
        };
        UpdatedAt = DateTime.UtcNow;
    }

    public void ResetLoginAttempts()
    {
        FailedLoginAttempts = 0;
        LockoutUntil = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsLockedOut() =>
        LockoutUntil.HasValue && LockoutUntil.Value > DateTime.UtcNow;

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RequirePasswordChange()
    {
        MustChangePassword = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
