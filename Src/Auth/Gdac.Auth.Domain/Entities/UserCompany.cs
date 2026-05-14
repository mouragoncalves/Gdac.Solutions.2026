namespace Gdac.Auth.Domain.Entities;

public class UserCompany
{
    public Guid UserId { get; private set; }
    public Guid CompanyId { get; private set; }

    public User User { get; private set; } = default!;
    public Company Company { get; private set; } = default!;

    private UserCompany() { }

    public static UserCompany Create(Guid userId, Guid companyId)
    {
        return new UserCompany
        {
            UserId = userId,
            CompanyId = companyId
        };
    }
}
