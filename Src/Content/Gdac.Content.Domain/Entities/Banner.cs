namespace Gdac.Content.Domain.Entities;

public class Banner
{
    public Guid    Id                   { get; private set; }
    public string  Title                { get; private set; } = default!;
    public string? Subtitle             { get; private set; }
    public string  ImageUrl             { get; private set; } = default!;
    public string  CtaText              { get; private set; } = default!;
    public string  CtaUrl               { get; private set; } = default!;
    public string? SecondaryCtaText     { get; private set; }
    public string? SecondaryCtaUrl      { get; private set; }
    public bool    IsActive             { get; private set; }
    public int     DisplayOrder         { get; private set; }
    public Guid?   PartnerId            { get; private set; }
    public DateTime CreatedAt           { get; private set; }
    public DateTime UpdatedAt           { get; private set; }

    private Banner() { }

    public static Banner Create(
        string title, string? subtitle,
        string imageUrl, string ctaText, string ctaUrl,
        string? secondaryCtaText, string? secondaryCtaUrl,
        Guid? partnerId)
    {
        return new Banner
        {
            Id               = Guid.NewGuid(),
            Title            = title.Trim(),
            Subtitle         = subtitle?.Trim(),
            ImageUrl         = imageUrl.Trim(),
            CtaText          = ctaText.Trim(),
            CtaUrl           = ctaUrl.Trim(),
            SecondaryCtaText = secondaryCtaText?.Trim(),
            SecondaryCtaUrl  = secondaryCtaUrl?.Trim(),
            IsActive         = true,
            DisplayOrder     = 0,
            PartnerId        = partnerId,
            CreatedAt        = DateTime.UtcNow,
            UpdatedAt        = DateTime.UtcNow
        };
    }

    public void Update(
        string title, string? subtitle,
        string imageUrl, string ctaText, string ctaUrl,
        string? secondaryCtaText, string? secondaryCtaUrl)
    {
        Title            = title.Trim();
        Subtitle         = subtitle?.Trim();
        ImageUrl         = imageUrl.Trim();
        CtaText          = ctaText.Trim();
        CtaUrl           = ctaUrl.Trim();
        SecondaryCtaText = secondaryCtaText?.Trim();
        SecondaryCtaUrl  = secondaryCtaUrl?.Trim();
        UpdatedAt        = DateTime.UtcNow;
    }

    public void SetActive(bool isActive) { IsActive = isActive; UpdatedAt = DateTime.UtcNow; }
    public void SetOrder(int order)      { DisplayOrder = order; UpdatedAt = DateTime.UtcNow; }
}
