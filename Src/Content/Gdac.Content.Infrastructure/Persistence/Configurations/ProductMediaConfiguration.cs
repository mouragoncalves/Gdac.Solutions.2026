using Gdac.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Content.Infrastructure.Persistence.Configurations;

public class ProductMediaConfiguration : IEntityTypeConfiguration<ProductMedia>
{
    public void Configure(EntityTypeBuilder<ProductMedia> builder)
    {
        builder.ToTable("content_product_media");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.ProductId).IsRequired();
        builder.Property(m => m.Url).IsRequired().HasMaxLength(500);
        builder.Property(m => m.Type).IsRequired().HasConversion<int>();
        builder.Property(m => m.DisplayOrder).IsRequired();
        builder.Property(m => m.CreatedAt).IsRequired();
    }
}
