using Gdac.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Content.Infrastructure.Persistence.Configurations;

public class BannerConfiguration : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.ToTable("content_banners");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();

        builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Subtitle).HasMaxLength(300);
        builder.Property(b => b.ImageUrl).IsRequired().HasMaxLength(500);
        builder.Property(b => b.CtaText).IsRequired().HasMaxLength(100);
        builder.Property(b => b.CtaUrl).IsRequired().HasMaxLength(500);
        builder.Property(b => b.SecondaryCtaText).HasMaxLength(100);
        builder.Property(b => b.SecondaryCtaUrl).HasMaxLength(500);
        builder.Property(b => b.IsActive).IsRequired();
        builder.Property(b => b.DisplayOrder).IsRequired();
        builder.Property(b => b.CreatedAt).IsRequired();
        builder.Property(b => b.UpdatedAt).IsRequired();
    }
}
