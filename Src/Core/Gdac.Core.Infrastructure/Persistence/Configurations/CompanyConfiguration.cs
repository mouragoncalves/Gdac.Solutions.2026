using Gdac.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Core.Infrastructure.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("core_companies");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.TradeName).HasMaxLength(200);

        builder.Property(c => c.CnpjBase).HasMaxLength(8);
        builder.HasIndex(c => c.CnpjBase).IsUnique().HasFilter("cnpj_base IS NOT NULL");

        builder.Property(c => c.Cnpj).HasMaxLength(18);
        builder.HasIndex(c => c.Cnpj).IsUnique().HasFilter("cnpj IS NOT NULL");

        builder.Property(c => c.Type).IsRequired().HasConversion<int>();
        builder.Property(c => c.Status).IsRequired().HasConversion<int>();

        builder.Property(c => c.Email).HasMaxLength(255);
        builder.Property(c => c.Phone).HasMaxLength(20);

        builder.Property(c => c.NatureText).HasMaxLength(100);
        builder.Property(c => c.SizeAcronym).HasMaxLength(10);
        builder.Property(c => c.SizeText).HasMaxLength(50);
        builder.Property(c => c.Equity).HasPrecision(18, 2);
        builder.Property(c => c.Jurisdiction).HasMaxLength(2);

        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();

        builder.HasMany(c => c.UserLinks)
            .WithOne(l => l.Company)
            .HasForeignKey(l => l.CompanyId);

        builder.HasMany(c => c.Members)
            .WithOne(m => m.Company)
            .HasForeignKey(m => m.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Offices)
            .WithOne(o => o.Company)
            .HasForeignKey(o => o.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
