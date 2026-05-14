using Gdac.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Core.Infrastructure.Persistence.Configurations;

public class UserCompanyLinkConfiguration : IEntityTypeConfiguration<UserCompanyLink>
{
    public void Configure(EntityTypeBuilder<UserCompanyLink> builder)
    {
        builder.ToTable("core_user_company_links");

        builder.HasKey(l => new { l.UserId, l.CompanyId });

        builder.Property(l => l.Role).IsRequired().HasMaxLength(50);
        builder.Property(l => l.IsActive).IsRequired();
        builder.Property(l => l.JoinedAt).IsRequired();
    }
}
