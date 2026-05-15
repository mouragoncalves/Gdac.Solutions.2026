using Gdac.Onboarding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Onboarding.Infrastructure.Persistence.Configurations;

public class LeadDistributionConfigConfiguration : IEntityTypeConfiguration<LeadDistributionConfig>
{
    public void Configure(EntityTypeBuilder<LeadDistributionConfig> builder)
    {
        builder.ToTable("onboarding_lead_distribution_config");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Mode).IsRequired().HasConversion<int>();
        builder.Property(c => c.UpdatedAt).IsRequired();
        builder.Property(c => c.UpdatedBy).IsRequired();
    }
}
