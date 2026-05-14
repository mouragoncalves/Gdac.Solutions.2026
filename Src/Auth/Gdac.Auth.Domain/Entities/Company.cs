namespace Gdac.Auth.Domain.Entities;

public class Company
{
    public Guid Id { get; private set; }
    public string ExternalId { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; }

    public ICollection<UserCompany> Users { get; private set; } = [];

    private Company() { }

    public static Company Create(string externalId, string name)
    {
        return new Company
        {
            Id = Guid.NewGuid(),
            ExternalId = externalId.Trim(),
            Name = name.Trim(),
            IsActive = true
        };
    }

    public void Update(string name)
    {
        Name = name.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
