namespace Gdac.Content.Domain.Entities;

public class ProductPriceHistory
{
    public Guid    Id                        { get; private set; }
    public Guid    ProductId                 { get; private set; }
    public decimal PrecoRevenda              { get; private set; }
    public decimal PrecoSugeridoFinal        { get; private set; }
    public decimal DescontoSugeridoSemestral { get; private set; }
    public decimal DescontoSugeridoAnual     { get; private set; }
    public Guid    ChangedBy                 { get; private set; }
    public DateTime ChangedAt               { get; private set; }
    public string? Notes                    { get; private set; }

    public Product Product { get; private set; } = default!;

    private ProductPriceHistory() { }

    public static ProductPriceHistory Create(
        Guid productId,
        decimal precoRevenda, decimal precoSugeridoFinal,
        decimal descontoSemestral, decimal descontoAnual,
        Guid changedBy, string? notes)
    {
        return new ProductPriceHistory
        {
            Id                        = Guid.NewGuid(),
            ProductId                 = productId,
            PrecoRevenda              = precoRevenda,
            PrecoSugeridoFinal        = precoSugeridoFinal,
            DescontoSugeridoSemestral = descontoSemestral,
            DescontoSugeridoAnual     = descontoAnual,
            ChangedBy                 = changedBy,
            ChangedAt                 = DateTime.UtcNow,
            Notes                     = notes?.Trim()
        };
    }
}
