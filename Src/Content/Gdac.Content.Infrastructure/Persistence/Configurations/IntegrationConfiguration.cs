using Gdac.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Content.Infrastructure.Persistence.Configurations;

public class IntegrationConfiguration : IEntityTypeConfiguration<Integration>
{
    public void Configure(EntityTypeBuilder<Integration> builder)
    {
        builder.ToTable("content_integrations");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();

        builder.Property(i => i.Name).IsRequired().HasMaxLength(200);
        builder.Property(i => i.Category).IsRequired().HasConversion<int>();
        builder.Property(i => i.LogoUrl).IsRequired().HasMaxLength(500);
        builder.Property(i => i.Description).IsRequired().HasColumnType("text");
        builder.Property(i => i.ExternalUrl).HasMaxLength(500);
        builder.Property(i => i.IsActive).IsRequired();
        builder.Property(i => i.DisplayOrder).IsRequired();
        builder.Property(i => i.CreatedAt).IsRequired();
        builder.Property(i => i.UpdatedAt).IsRequired();
    }
}
