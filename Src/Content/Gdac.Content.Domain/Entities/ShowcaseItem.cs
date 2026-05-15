using Gdac.Content.Domain.Enums;

namespace Gdac.Content.Domain.Entities;

public class ShowcaseItem
{
    public Guid            Id            { get; private set; }
    public ShowcaseItemType Type          { get; private set; }
    public Guid            CoreCompanyId { get; private set; }
    public string          Name          { get; private set; } = default!;
    public string?         LogoUrl       { get; private set; }
    public bool            IsActive      { get; private set; }
    public int             DisplayOrder  { get; private set; }
    public DateTime        CreatedAt     { get; private set; }
    public DateTime        UpdatedAt     { get; private set; }

    private ShowcaseItem() { }

    public static ShowcaseItem Create(
        ShowcaseItemType type, Guid coreCompanyId,
        string name, string? logoUrl)
    {
        return new ShowcaseItem
        {
            Id            = Guid.NewGuid(),
            Type          = type,
            CoreCompanyId = coreCompanyId,
            Name          = name.Trim(),
            LogoUrl       = logoUrl?.Trim(),
            IsActive      = true,
            DisplayOrder  = 0,
            CreatedAt     = DateTime.UtcNow,
            UpdatedAt     = DateTime.UtcNow
        };
    }

    public void Update(ShowcaseItemType type, Guid coreCompanyId, string name, string? logoUrl)
    {
        Type          = type;
        CoreCompanyId = coreCompanyId;
        Name          = name.Trim();
        LogoUrl       = logoUrl?.Trim();
        UpdatedAt     = DateTime.UtcNow;
    }

    public void SetActive(bool isActive) { IsActive = isActive; UpdatedAt = DateTime.UtcNow; }
    public void SetOrder(int order)      { DisplayOrder = order; UpdatedAt = DateTime.UtcNow; }
}
