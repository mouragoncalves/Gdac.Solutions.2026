namespace Gdac.Core.Domain.Entities;

public class UserCompanyLink
{
    public Guid UserId { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Role { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public UserProfile User { get; private set; } = default!;
    public Company Company { get; private set; } = default!;

    private UserCompanyLink() { }

    public static UserCompanyLink Create(Guid userId, Guid companyId, string role)
    {
        return new UserCompanyLink
        {
            UserId    = userId,
            CompanyId = companyId,
            Role      = role.Trim().ToLowerInvariant(),
            IsActive  = true,
            JoinedAt  = DateTime.UtcNow
        };
    }

    public void Deactivate() => IsActive = false;
    public void UpdateRole(string role) => Role = role.Trim().ToLowerInvariant();
}
