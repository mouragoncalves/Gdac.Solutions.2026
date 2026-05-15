namespace Gdac.Content.Domain.Entities;

public class ContentService
{
    public Guid    Id                        { get; private set; }
    public string  Name                      { get; private set; } = default!;
    public string  Category                  { get; private set; } = default!;
    public string  Description               { get; private set; } = default!;
    public bool    IsActive                  { get; private set; }
    public bool    IsFeatured                { get; private set; }
    public int     DisplayOrder              { get; private set; }
    public decimal PrecoRevenda              { get; private set; }
    public decimal PrecoSugeridoFinal        { get; private set; }
    public decimal DescontoSugeridoSemestral { get; private set; }
    public decimal DescontoSugeridoAnual     { get; private set; }
    public DateTime CreatedAt               { get; private set; }
    public DateTime UpdatedAt               { get; private set; }

    public ICollection<ServiceMedia>        Media        { get; private set; } = [];
    public ICollection<ServicePriceHistory> PriceHistory { get; private set; } = [];

    private ContentService() { }

    public static ContentService Create(
        string name, string category, string description,
        decimal precoRevenda, decimal precoSugeridoFinal,
        decimal descontoSemestral = 10m, decimal descontoAnual = 25m)
    {
        return new ContentService
        {
            Id                        = Guid.NewGuid(),
            Name                      = name.Trim(),
            Category                  = category.Trim(),
            Description               = description.Trim(),
            IsActive                  = true,
            IsFeatured                = false,
            DisplayOrder              = 0,
            PrecoRevenda              = precoRevenda,
            PrecoSugeridoFinal        = precoSugeridoFinal,
            DescontoSugeridoSemestral = descontoSemestral,
            DescontoSugeridoAnual     = descontoAnual,
            CreatedAt                 = DateTime.UtcNow,
            UpdatedAt                 = DateTime.UtcNow
        };
    }

    public void UpdateInfo(string name, string category, string description, bool isFeatured, int displayOrder)
    {
        Name         = name.Trim();
        Category     = category.Trim();
        Description  = description.Trim();
        IsFeatured   = isFeatured;
        DisplayOrder = displayOrder;
        UpdatedAt    = DateTime.UtcNow;
    }

    public void UpdatePricing(
        decimal precoRevenda, decimal precoSugeridoFinal,
        decimal descontoSemestral, decimal descontoAnual,
        Guid changedBy, string? notes)
    {
        var history = ServicePriceHistory.Create(
            Id, precoRevenda, precoSugeridoFinal,
            descontoSemestral, descontoAnual, changedBy, notes);

        PriceHistory.Add(history);

        PrecoRevenda              = precoRevenda;
        PrecoSugeridoFinal        = precoSugeridoFinal;
        DescontoSugeridoSemestral = descontoSemestral;
        DescontoSugeridoAnual     = descontoAnual;
        UpdatedAt                 = DateTime.UtcNow;
    }

    public void SetActive(bool isActive) { IsActive = isActive; UpdatedAt = DateTime.UtcNow; }
}
