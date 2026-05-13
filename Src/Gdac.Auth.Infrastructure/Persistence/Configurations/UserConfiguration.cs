using Gdac.Auth.Domain.Entities;
using Gdac.Auth.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Auth.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);
        builder.Property(u => u.PasswordAlgorithm).IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.IsActive).IsRequired();
        builder.Property(u => u.MustChangePassword).IsRequired();
        builder.Property(u => u.FailedLoginAttempts).IsRequired().HasDefaultValue(0);
        builder.Property(u => u.LockoutUntil).IsRequired(false);
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired();
    }
}
