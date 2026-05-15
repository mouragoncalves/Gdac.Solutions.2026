using Gdac.Content.Domain.Enums;

namespace Gdac.Content.Domain.Entities;

public class Integration
{
    public Guid                Id           { get; private set; }
    public string              Name         { get; private set; } = default!;
    public IntegrationCategory Category     { get; private set; }
    public string              LogoUrl      { get; private set; } = default!;
    public string              Description  { get; private set; } = default!;
    public string?             ExternalUrl  { get; private set; }
    public bool                IsActive     { get; private set; }
    public int                 DisplayOrder { get; private set; }
    public DateTime            CreatedAt    { get; private set; }
    public DateTime            UpdatedAt    { get; private set; }

    private Integration() { }

    public static Integration Create(
        string name, IntegrationCategory category,
        string logoUrl, string description, string? externalUrl)
    {
        return new Integration
        {
            Id          = Guid.NewGuid(),
            Name        = name.Trim(),
            Category    = category,
            LogoUrl     = logoUrl.Trim(),
            Description = description.Trim(),
            ExternalUrl = externalUrl?.Trim(),
            IsActive    = true,
            DisplayOrder = 0,
            CreatedAt   = DateTime.UtcNow,
            UpdatedAt   = DateTime.UtcNow
        };
    }

    public void Update(
        string name, IntegrationCategory category,
        string logoUrl, string description, string? externalUrl)
    {
        Name        = name.Trim();
        Category    = category;
        LogoUrl     = logoUrl.Trim();
        Description = description.Trim();
        ExternalUrl = externalUrl?.Trim();
        UpdatedAt   = DateTime.UtcNow;
    }

    public void SetActive(bool isActive) { IsActive = isActive; UpdatedAt = DateTime.UtcNow; }
    public void SetOrder(int order)      { DisplayOrder = order; UpdatedAt = DateTime.UtcNow; }
}
