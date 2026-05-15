using Gdac.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Content.Infrastructure.Persistence.Configurations;

public class ContentServiceConfiguration : IEntityTypeConfiguration<ContentService>
{
    public void Configure(EntityTypeBuilder<ContentService> builder)
    {
        builder.ToTable("content_services");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Category).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Description).IsRequired().HasColumnType("mediumtext");
        builder.Property(s => s.IsActive).IsRequired();
        builder.Property(s => s.IsFeatured).IsRequired();
        builder.Property(s => s.DisplayOrder).IsRequired();
        builder.Property(s => s.PrecoRevenda).IsRequired().HasPrecision(18, 2);
        builder.Property(s => s.PrecoSugeridoFinal).IsRequired().HasPrecision(18, 2);
        builder.Property(s => s.DescontoSugeridoSemestral).IsRequired().HasPrecision(5, 2);
        builder.Property(s => s.DescontoSugeridoAnual).IsRequired().HasPrecision(5, 2);
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt).IsRequired();

        builder.HasMany(s => s.Media)
            .WithOne(m => m.Service)
            .HasForeignKey(m => m.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.PriceHistory)
            .WithOne(h => h.Service)
            .HasForeignKey(h => h.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
