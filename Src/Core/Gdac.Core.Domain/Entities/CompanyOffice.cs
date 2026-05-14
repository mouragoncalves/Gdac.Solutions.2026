namespace Gdac.Core.Domain.Entities;

public class CompanyOffice
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; } = default!;

    // Full 14-digit CNPJ of this office (without formatting)
    public string TaxId { get; private set; } = default!;
    // Nome fantasia
    public string? Alias { get; private set; }
    public DateOnly? Founded { get; private set; }
    // true = matriz, false = filial
    public bool IsHead { get; private set; }

    // Situação cadastral (e.g. 2=Ativa, 3=Suspensa, 4=Inapta, 8=Baixada)
    public int StatusId { get; private set; }
    public string StatusText { get; private set; } = default!;
    public DateOnly? StatusDate { get; private set; }

    // Motivo da situação
    public int? ReasonId { get; private set; }
    public string? ReasonText { get; private set; }

    // CNAE principal
    public int? MainActivityId { get; private set; }
    public string? MainActivityText { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private CompanyOffice() { }

    public static CompanyOffice Create(
        Guid companyId,
        string taxId,
        int statusId,
        string statusText,
        bool isHead = false,
        string? alias = null,
        DateOnly? founded = null,
        DateOnly? statusDate = null,
        int? reasonId = null,
        string? reasonText = null,
        int? mainActivityId = null,
        string? mainActivityText = null)
    {
        return new CompanyOffice
        {
            Id               = Guid.NewGuid(),
            CompanyId        = companyId,
            TaxId            = taxId.Trim(),
            Alias            = alias?.Trim(),
            Founded          = founded,
            IsHead           = isHead,
            StatusId         = statusId,
            StatusText       = statusText.Trim(),
            StatusDate       = statusDate,
            ReasonId         = reasonId,
            ReasonText       = reasonText?.Trim(),
            MainActivityId   = mainActivityId,
            MainActivityText = mainActivityText?.Trim(),
            CreatedAt        = DateTime.UtcNow
        };
    }
}
