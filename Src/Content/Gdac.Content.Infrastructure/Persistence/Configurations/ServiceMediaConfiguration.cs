using Gdac.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Content.Infrastructure.Persistence.Configurations;

public class ServiceMediaConfiguration : IEntityTypeConfiguration<ServiceMedia>
{
    public void Configure(EntityTypeBuilder<ServiceMedia> builder)
    {
        builder.ToTable("content_service_media");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.ServiceId).IsRequired();
        builder.Property(m => m.Url).IsRequired().HasMaxLength(500);
        builder.Property(m => m.Type).IsRequired().HasConversion<int>();
        builder.Property(m => m.DisplayOrder).IsRequired();
        builder.Property(m => m.CreatedAt).IsRequired();
    }
}
