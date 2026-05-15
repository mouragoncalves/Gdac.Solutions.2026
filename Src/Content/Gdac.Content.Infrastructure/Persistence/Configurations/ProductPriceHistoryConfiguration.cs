using Gdac.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Content.Infrastructure.Persistence.Configurations;

public class ProductPriceHistoryConfiguration : IEntityTypeConfiguration<ProductPriceHistory>
{
    public void Configure(EntityTypeBuilder<ProductPriceHistory> builder)
    {
        builder.ToTable("content_product_price_history");

        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();

        builder.Property(h => h.ProductId).IsRequired();
        builder.Property(h => h.PrecoRevenda).IsRequired().HasPrecision(18, 2);
        builder.Property(h => h.PrecoSugeridoFinal).IsRequired().HasPrecision(18, 2);
        builder.Property(h => h.DescontoSugeridoSemestral).IsRequired().HasPrecision(5, 2);
        builder.Property(h => h.DescontoSugeridoAnual).IsRequired().HasPrecision(5, 2);
        builder.Property(h => h.ChangedBy).IsRequired();
        builder.Property(h => h.ChangedAt).IsRequired();
        builder.Property(h => h.Notes).HasMaxLength(500);
    }
}
