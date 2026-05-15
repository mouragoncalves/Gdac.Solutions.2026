namespace Gdac.Content.Domain.Entities;

public class Testimonial
{
    public Guid    Id              { get; private set; }
    public string  AuthorName      { get; private set; } = default!;
    public string? AuthorRole      { get; private set; }
    public string? AuthorCompany   { get; private set; }
    public string? AuthorPhotoUrl  { get; private set; }
    public string  Content         { get; private set; } = default!;
    public int     Rating          { get; private set; }
    public bool    IsActive        { get; private set; }
    public int     DisplayOrder    { get; private set; }
    public Guid?   PartnerId       { get; private set; }
    public DateTime CreatedAt      { get; private set; }
    public DateTime UpdatedAt      { get; private set; }

    private Testimonial() { }

    public static Testimonial Create(
        string authorName, string? authorRole, string? authorCompany,
        string? authorPhotoUrl, string content, int rating, Guid? partnerId)
    {
        return new Testimonial
        {
            Id             = Guid.NewGuid(),
            AuthorName     = authorName.Trim(),
            AuthorRole     = authorRole?.Trim(),
            AuthorCompany  = authorCompany?.Trim(),
            AuthorPhotoUrl = authorPhotoUrl?.Trim(),
            Content        = content.Trim(),
            Rating         = rating,
            IsActive       = true,
            DisplayOrder   = 0,
            PartnerId      = partnerId,
            CreatedAt      = DateTime.UtcNow,
            UpdatedAt      = DateTime.UtcNow
        };
    }

    public void Update(
        string authorName, string? authorRole, string? authorCompany,
        string? authorPhotoUrl, string content, int rating)
    {
        AuthorName     = authorName.Trim();
        AuthorRole     = authorRole?.Trim();
        AuthorCompany  = authorCompany?.Trim();
        AuthorPhotoUrl = authorPhotoUrl?.Trim();
        Content        = content.Trim();
        Rating         = rating;
        UpdatedAt      = DateTime.UtcNow;
    }

    public void SetActive(bool isActive) { IsActive = isActive; UpdatedAt = DateTime.UtcNow; }
    public void SetOrder(int order)      { DisplayOrder = order; UpdatedAt = DateTime.UtcNow; }
}
