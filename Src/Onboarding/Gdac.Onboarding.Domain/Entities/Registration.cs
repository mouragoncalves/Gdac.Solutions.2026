using Gdac.Onboarding.Domain.Enums;

namespace Gdac.Onboarding.Domain.Entities;

public class Registration
{
    public Guid Id { get; private set; }
    public RegistrationType Type { get; private set; }
    public RegistrationStatus Status { get; private set; }

    public string ContactName { get; private set; } = default!;
    public string ContactEmail { get; private set; } = default!;
    public string? ContactPhone { get; private set; }

    public string CompanyName { get; private set; } = default!;
    public string? TradeName { get; private set; }
    public string Cnpj { get; private set; } = default!;
    public string CnpjBase { get; private set; } = default!;
    public ClientSegment? Segment { get; private set; }
    public CompanySize? SizeCategory { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }

    public string? ReferralCode { get; private set; }
    public Guid? AssignedPartnerId { get; private set; }
    public LeadDistributionMode? DistributionMode { get; private set; }

    public string? ReviewNotes { get; private set; }
    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }

    public Guid? ExternalCompanyId { get; private set; }
    public Guid? ExternalUserId { get; private set; }

    public string? IpAddress { get; private set; }
    public DateTime SubmittedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Registration() { }

    public static Registration CreateClient(
        string contactName, string contactEmail, string? contactPhone,
        string companyName, string? tradeName, string cnpj,
        ClientSegment? segment, CompanySize? sizeCategory,
        string? city, string? state, string? referralCode,
        Guid? assignedPartnerId, LeadDistributionMode? distributionMode,
        string? ipAddress)
    {
        return new Registration
        {
            Id = Guid.NewGuid(),
            Type = RegistrationType.Client,
            Status = RegistrationStatus.Pending,
            ContactName = contactName.Trim(),
            ContactEmail = contactEmail.Trim().ToLowerInvariant(),
            ContactPhone = contactPhone?.Trim(),
            CompanyName = companyName.Trim(),
            TradeName = tradeName?.Trim(),
            Cnpj = cnpj.Trim(),
            CnpjBase = cnpj.Trim().Substring(0, 8),
            Segment = segment,
            SizeCategory = sizeCategory,
            City = city?.Trim(),
            State = state?.Trim().ToUpperInvariant(),
            ReferralCode = referralCode?.Trim().ToUpperInvariant(),
            AssignedPartnerId = assignedPartnerId,
            DistributionMode = distributionMode,
            IpAddress = ipAddress,
            SubmittedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Registration CreatePartner(
        string contactName, string contactEmail, string? contactPhone,
        string companyName, string? tradeName, string cnpj,
        ClientSegment? segment, CompanySize? sizeCategory,
        string? city, string? state, string? ipAddress)
    {
        return new Registration
        {
            Id = Guid.NewGuid(),
            Type = RegistrationType.Partner,
            Status = RegistrationStatus.Pending,
            ContactName = contactName.Trim(),
            ContactEmail = contactEmail.Trim().ToLowerInvariant(),
            ContactPhone = contactPhone?.Trim(),
            CompanyName = companyName.Trim(),
            TradeName = tradeName?.Trim(),
            Cnpj = cnpj.Trim(),
            CnpjBase = cnpj.Trim().Substring(0, 8),
            Segment = segment,
            SizeCategory = sizeCategory,
            City = city?.Trim(),
            State = state?.Trim().ToUpperInvariant(),
            IpAddress = ipAddress,
            SubmittedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void SetUnderReview()
    {
        Status = RegistrationStatus.UnderReview;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(Guid reviewedBy, Guid externalCompanyId, Guid externalUserId, string? notes = null)
    {
        Status = RegistrationStatus.Approved;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = notes?.Trim();
        ExternalCompanyId = externalCompanyId;
        ExternalUserId = externalUserId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(Guid reviewedBy, string? notes = null)
    {
        Status = RegistrationStatus.Rejected;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Block(Guid reviewedBy, string? notes = null)
    {
        Status = RegistrationStatus.Blocked;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignPartner(Guid partnerId, LeadDistributionMode mode)
    {
        AssignedPartnerId = partnerId;
        DistributionMode = mode;
        UpdatedAt = DateTime.UtcNow;
    }
}
