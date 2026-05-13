using Gdac.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Auth.Infrastructure.Persistence.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.RefreshTokenHash).IsRequired().HasMaxLength(500);
        builder.Property(s => s.IpAddress).IsRequired().HasMaxLength(45);
        builder.Property(s => s.DeviceInfo).IsRequired().HasMaxLength(500);
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.LastActivityAt).IsRequired();
        builder.Property(s => s.AbsoluteExpiration).IsRequired();
        builder.Property(s => s.IsRevoked).IsRequired();

        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => new { s.IsRevoked, s.AbsoluteExpiration });

        builder.HasOne(s => s.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Application)
            .WithMany(a => a.Sessions)
            .HasForeignKey(s => s.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
