using Gdac.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Core.Infrastructure.Persistence.Configurations;

public class CompanyMemberConfiguration : IEntityTypeConfiguration<CompanyMember>
{
    public void Configure(EntityTypeBuilder<CompanyMember> builder)
    {
        builder.ToTable("core_company_members");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.PersonType).IsRequired().HasConversion<int>();
        builder.Property(m => m.PersonExternalId).HasMaxLength(36);
        builder.Property(m => m.PersonName).IsRequired().HasMaxLength(200);
        builder.Property(m => m.PersonTaxId).HasMaxLength(20);
        builder.Property(m => m.PersonAge).HasMaxLength(10);

        builder.Property(m => m.RoleId).IsRequired();
        builder.Property(m => m.RoleText).IsRequired().HasMaxLength(100);

        builder.Property(m => m.AgentName).HasMaxLength(200);
        builder.Property(m => m.AgentTaxId).HasMaxLength(20);
        builder.Property(m => m.AgentRoleText).HasMaxLength(100);

        builder.Property(m => m.CreatedAt).IsRequired();

        builder.HasIndex(m => m.CompanyId);
    }
}
