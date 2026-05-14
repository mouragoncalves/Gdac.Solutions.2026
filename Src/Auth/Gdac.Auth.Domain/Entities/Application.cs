namespace Gdac.Auth.Domain.Entities;

public class Application
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string ClientId { get; private set; } = default!;
    public string ClientSecretHash { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ICollection<UserApplication> Users { get; private set; } = [];
    public ICollection<Session> Sessions { get; private set; } = [];

    private Application() { }

    public static Application Create(string name, string clientId, string clientSecretHash)
    {
        return new Application
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            ClientId = clientId.Trim(),
            ClientSecretHash = clientSecretHash,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void RotateSecret(string newSecretHash)
    {
        ClientSecretHash = newSecretHash;
    }

    public void Update(string name)
    {
        Name = name.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
