using Gdac.Core.Domain.Enums;

namespace Gdac.Core.Domain.Entities;

public class Company
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string? TradeName { get; private set; }

    // Raw 8-digit CNPJ root (shared by all offices of this legal entity)
    public string? CnpjBase { get; private set; }
    // Full formatted CNPJ (typically the head-office one, e.g. "12.345.678/0001-90")
    public string? Cnpj { get; private set; }

    public CompanyType Type { get; private set; }
    public CompanyStatus Status { get; private set; }

    public ClientSegment? Segment { get; private set; }
    public CompanySize? SizeCategory { get; private set; }

    public string? Email { get; private set; }
    public string? Phone { get; private set; }

    // Natureza jurídica (e.g. 2062 = "Sociedade Anônima Aberta")
    public int? NatureId { get; private set; }
    public string? NatureText { get; private set; }

    // Porte (e.g. 5 = "DEMAIS")
    public int? SizeId { get; private set; }
    public string? SizeAcronym { get; private set; }  // "ME", "EPP", "DEMAIS"
    public string? SizeText { get; private set; }

    // Capital social
    public decimal? Equity { get; private set; }

    // UF de jurisdição (registro)
    public string? Jurisdiction { get; private set; }

    // Simples Nacional
    public bool SimplesOptant { get; private set; }
    public DateOnly? SimplesSince { get; private set; }

    // SIMEI (MEI)
    public bool SimeiOptant { get; private set; }
    public DateOnly? SimeiSince { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public ICollection<UserCompanyLink> UserLinks { get; private set; } = [];
    public ICollection<CompanyMember> Members { get; private set; } = [];
    public ICollection<CompanyOffice> Offices { get; private set; } = [];

    private Company() { }

    public static Company Create(string name, CompanyType type, string? tradeName = null,
        string? cnpj = null, string? email = null, string? phone = null,
        ClientSegment? segment = null, CompanySize? sizeCategory = null)
    {
        return new Company
        {
            Id           = Guid.NewGuid(),
            Name         = name.Trim(),
            TradeName    = tradeName?.Trim(),
            Cnpj         = cnpj?.Trim(),
            Type         = type,
            Status       = CompanyStatus.Prospect,
            Segment      = segment,
            SizeCategory = sizeCategory,
            Email        = email?.Trim().ToLowerInvariant(),
            Phone        = phone?.Trim(),
            CreatedAt    = DateTime.UtcNow,
            UpdatedAt    = DateTime.UtcNow
        };
    }

    public void Update(string name, string? tradeName, string? cnpj,
        CompanyType type, string? email, string? phone,
        ClientSegment? segment, CompanySize? sizeCategory)
    {
        Name         = name.Trim();
        TradeName    = tradeName?.Trim();
        Cnpj         = cnpj?.Trim();
        Type         = type;
        Email        = email?.Trim().ToLowerInvariant();
        Phone        = phone?.Trim();
        Segment      = segment;
        SizeCategory = sizeCategory;
        UpdatedAt    = DateTime.UtcNow;
    }

    // Populated from CNPJ API data (Receita Federal)
    public void SetCnpjData(
        string? cnpjBase,
        int? natureId, string? natureText,
        int? sizeId, string? sizeAcronym, string? sizeText,
        decimal? equity, string? jurisdiction,
        bool simplesOptant, DateOnly? simplesSince,
        bool simeiOptant, DateOnly? simeiSince)
    {
        CnpjBase      = cnpjBase?.Trim();
        NatureId      = natureId;
        NatureText    = natureText?.Trim();
        SizeId        = sizeId;
        SizeAcronym   = sizeAcronym?.Trim();
        SizeText      = sizeText?.Trim();
        Equity        = equity;
        Jurisdiction  = jurisdiction?.Trim();
        SimplesOptant = simplesOptant;
        SimplesSince  = simplesSince;
        SimeiOptant   = simeiOptant;
        SimeiSince    = simeiSince;
        UpdatedAt     = DateTime.UtcNow;
    }

    public void SetStatus(CompanyStatus status) { Status = status; UpdatedAt = DateTime.UtcNow; }
}
