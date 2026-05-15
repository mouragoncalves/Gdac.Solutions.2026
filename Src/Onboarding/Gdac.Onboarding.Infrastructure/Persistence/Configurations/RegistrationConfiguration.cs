using Gdac.Onboarding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Onboarding.Infrastructure.Persistence.Configurations;

public class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
{
    public void Configure(EntityTypeBuilder<Registration> builder)
    {
        builder.ToTable("onboarding_registrations");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.Type).IsRequired().HasConversion<int>();
        builder.Property(r => r.Status).IsRequired().HasConversion<int>();

        builder.Property(r => r.ContactName).IsRequired().HasMaxLength(200);
        builder.Property(r => r.ContactEmail).IsRequired().HasMaxLength(255);
        builder.Property(r => r.ContactPhone).HasMaxLength(20);

        builder.Property(r => r.CompanyName).IsRequired().HasMaxLength(200);
        builder.Property(r => r.TradeName).HasMaxLength(200);
        builder.Property(r => r.Cnpj).IsRequired().HasMaxLength(14);
        builder.Property(r => r.CnpjBase).IsRequired().HasMaxLength(8);

        builder.Property(r => r.Segment).HasConversion<int?>();
        builder.Property(r => r.SizeCategory).HasConversion<int?>();
        builder.Property(r => r.City).HasMaxLength(100);
        builder.Property(r => r.State).HasMaxLength(2);

        builder.Property(r => r.ReferralCode).HasMaxLength(50);
        builder.Property(r => r.DistributionMode).HasConversion<int?>();

        builder.Property(r => r.ReviewNotes).HasMaxLength(1000);
        builder.Property(r => r.IpAddress).HasMaxLength(45);

        builder.Property(r => r.SubmittedAt).IsRequired();
        builder.Property(r => r.UpdatedAt).IsRequired();

        builder.HasIndex(r => r.Cnpj);
        builder.HasIndex(r => r.ContactEmail);
        builder.HasIndex(r => r.Status);
    }
}
