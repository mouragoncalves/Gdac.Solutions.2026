namespace Gdac.Auth.Domain.Entities;

public class UserApplication
{
    public Guid UserId { get; private set; }
    public Guid ApplicationId { get; private set; }

    public User User { get; private set; } = default!;
    public Application Application { get; private set; } = default!;

    private UserApplication() { }

    public static UserApplication Create(Guid userId, Guid applicationId)
    {
        return new UserApplication
        {
            UserId = userId,
            ApplicationId = applicationId
        };
    }
}
