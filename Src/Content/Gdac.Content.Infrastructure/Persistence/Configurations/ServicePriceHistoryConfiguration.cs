using Gdac.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Content.Infrastructure.Persistence.Configurations;

public class ServicePriceHistoryConfiguration : IEntityTypeConfiguration<ServicePriceHistory>
{
    public void Configure(EntityTypeBuilder<ServicePriceHistory> builder)
    {
        builder.ToTable("content_service_price_history");

        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();

        builder.Property(h => h.ServiceId).IsRequired();
        builder.Property(h => h.PrecoRevenda).IsRequired().HasPrecision(18, 2);
        builder.Property(h => h.PrecoSugeridoFinal).IsRequired().HasPrecision(18, 2);
        builder.Property(h => h.DescontoSugeridoSemestral).IsRequired().HasPrecision(5, 2);
        builder.Property(h => h.DescontoSugeridoAnual).IsRequired().HasPrecision(5, 2);
        builder.Property(h => h.ChangedBy).IsRequired();
        builder.Property(h => h.ChangedAt).IsRequired();
        builder.Property(h => h.Notes).HasMaxLength(500);
    }
}
