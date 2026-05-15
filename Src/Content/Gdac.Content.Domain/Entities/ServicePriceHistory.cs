namespace Gdac.Content.Domain.Entities;

public class ServicePriceHistory
{
    public Guid    Id                        { get; private set; }
    public Guid    ServiceId                 { get; private set; }
    public decimal PrecoRevenda              { get; private set; }
    public decimal PrecoSugeridoFinal        { get; private set; }
    public decimal DescontoSugeridoSemestral { get; private set; }
    public decimal DescontoSugeridoAnual     { get; private set; }
    public Guid    ChangedBy                 { get; private set; }
    public DateTime ChangedAt               { get; private set; }
    public string? Notes                    { get; private set; }

    public ContentService Service { get; private set; } = default!;

    private ServicePriceHistory() { }

    public static ServicePriceHistory Create(
        Guid serviceId,
        decimal precoRevenda, decimal precoSugeridoFinal,
        decimal descontoSemestral, decimal descontoAnual,
        Guid changedBy, string? notes)
    {
        return new ServicePriceHistory
        {
            Id                        = Guid.NewGuid(),
            ServiceId                 = serviceId,
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
