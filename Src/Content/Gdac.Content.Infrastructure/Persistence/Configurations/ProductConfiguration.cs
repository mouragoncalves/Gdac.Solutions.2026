using Gdac.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Content.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("content_products");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Category).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description).IsRequired().HasColumnType("mediumtext");
        builder.Property(p => p.IsActive).IsRequired();
        builder.Property(p => p.IsFeatured).IsRequired();
        builder.Property(p => p.DisplayOrder).IsRequired();
        builder.Property(p => p.PrecoRevenda).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.PrecoSugeridoFinal).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.DescontoSugeridoSemestral).IsRequired().HasPrecision(5, 2);
        builder.Property(p => p.DescontoSugeridoAnual).IsRequired().HasPrecision(5, 2);
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();

        builder.HasMany(p => p.Media)
            .WithOne(m => m.Product)
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.PriceHistory)
            .WithOne(h => h.Product)
            .HasForeignKey(h => h.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
