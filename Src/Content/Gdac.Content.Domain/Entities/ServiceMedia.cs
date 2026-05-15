using Gdac.Content.Domain.Enums;

namespace Gdac.Content.Domain.Entities;

public class ServiceMedia
{
    public Guid      Id           { get; private set; }
    public Guid      ServiceId    { get; private set; }
    public string    Url          { get; private set; } = default!;
    public MediaType Type         { get; private set; }
    public int       DisplayOrder { get; private set; }
    public DateTime  CreatedAt    { get; private set; }

    public ContentService Service { get; private set; } = default!;

    private ServiceMedia() { }

    public static ServiceMedia Create(Guid serviceId, string url, MediaType type, int displayOrder)
    {
        return new ServiceMedia
        {
            Id           = Guid.NewGuid(),
            ServiceId    = serviceId,
            Url          = url.Trim(),
            Type         = type,
            DisplayOrder = displayOrder,
            CreatedAt    = DateTime.UtcNow
        };
    }
}
