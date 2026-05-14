using Gdac.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Core.Infrastructure.Persistence.Configurations;

public class CompanyOfficeConfiguration : IEntityTypeConfiguration<CompanyOffice>
{
    public void Configure(EntityTypeBuilder<CompanyOffice> builder)
    {
        builder.ToTable("core_company_offices");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        builder.Property(o => o.TaxId).IsRequired().HasMaxLength(14);
        builder.HasIndex(o => o.TaxId).IsUnique();

        builder.Property(o => o.Alias).HasMaxLength(200);

        builder.Property(o => o.StatusId).IsRequired();
        builder.Property(o => o.StatusText).IsRequired().HasMaxLength(50);
        builder.Property(o => o.ReasonText).HasMaxLength(100);

        builder.Property(o => o.MainActivityText).HasMaxLength(200);

        builder.Property(o => o.CreatedAt).IsRequired();

        builder.HasIndex(o => o.CompanyId);
    }
}
