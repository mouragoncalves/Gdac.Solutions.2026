using Gdac.Content.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Content.Infrastructure.Persistence.Configurations;

public class TestimonialConfiguration : IEntityTypeConfiguration<Testimonial>
{
    public void Configure(EntityTypeBuilder<Testimonial> builder)
    {
        builder.ToTable("content_testimonials");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();

        builder.Property(t => t.AuthorName).IsRequired().HasMaxLength(200);
        builder.Property(t => t.AuthorRole).HasMaxLength(200);
        builder.Property(t => t.AuthorCompany).HasMaxLength(200);
        builder.Property(t => t.AuthorPhotoUrl).HasMaxLength(500);
        builder.Property(t => t.Content).IsRequired().HasColumnType("text");
        builder.Property(t => t.Rating).IsRequired();
        builder.Property(t => t.IsActive).IsRequired();
        builder.Property(t => t.DisplayOrder).IsRequired();
        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.UpdatedAt).IsRequired();
    }
}
