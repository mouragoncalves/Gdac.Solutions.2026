namespace Gdac.Core.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? Phone { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string? Cpf { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public ICollection<UserCompanyLink> CompanyLinks { get; private set; } = [];

    private UserProfile() { }

    public static UserProfile Create(Guid id, string fullName, string email)
    {
        return new UserProfile
        {
            Id        = id,
            FullName  = fullName.Trim(),
            Email     = email.ToLowerInvariant().Trim(),
            IsActive  = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string fullName, string? phone, string? avatarUrl, string? cpf, DateOnly? birthDate)
    {
        FullName  = fullName.Trim();
        Phone     = phone?.Trim();
        AvatarUrl = avatarUrl?.Trim();
        Cpf       = cpf?.Trim();
        BirthDate = birthDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
    public void Activate()   { IsActive = true;  UpdatedAt = DateTime.UtcNow; }
}
