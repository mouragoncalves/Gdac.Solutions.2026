using Gdac.Onboarding.Domain.Enums;

namespace Gdac.Onboarding.Domain.Entities;

public class LeadDistributionConfig
{
    public Guid Id { get; private set; }
    public LeadDistributionMode Mode { get; private set; }
    public Guid? DefaultPartnerId { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid UpdatedBy { get; private set; }

    private LeadDistributionConfig() { }

    public static LeadDistributionConfig CreateDefault()
    {
        return new LeadDistributionConfig
        {
            Id = Guid.NewGuid(),
            Mode = LeadDistributionMode.Manual,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = Guid.Empty
        };
    }

    public void Update(LeadDistributionMode mode, Guid? defaultPartnerId, Guid updatedBy)
    {
        Mode = mode;
        DefaultPartnerId = defaultPartnerId;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }
}
