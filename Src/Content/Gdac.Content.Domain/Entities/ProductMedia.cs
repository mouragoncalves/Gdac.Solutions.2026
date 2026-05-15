using Gdac.Content.Domain.Enums;

namespace Gdac.Content.Domain.Entities;

public class ProductMedia
{
    public Guid      Id           { get; private set; }
    public Guid      ProductId    { get; private set; }
    public string    Url          { get; private set; } = default!;
    public MediaType Type         { get; private set; }
    public int       DisplayOrder { get; private set; }
    public DateTime  CreatedAt    { get; private set; }

    public Product Product { get; private set; } = default!;

    private ProductMedia() { }

    public static ProductMedia Create(Guid productId, string url, MediaType type, int displayOrder)
    {
        return new ProductMedia
        {
            Id           = Guid.NewGuid(),
            ProductId    = productId,
            Url          = url.Trim(),
            Type         = type,
            DisplayOrder = displayOrder,
            CreatedAt    = DateTime.UtcNow
        };
    }
}
