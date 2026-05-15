using Gdac.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gdac.Core.Infrastructure.Persistence.Configurations;

public class BlockRecordConfiguration : IEntityTypeConfiguration<BlockRecord>
{
    public void Configure(EntityTypeBuilder<BlockRecord> builder)
    {
        builder.ToTable("core_block_records");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.CnpjBase).HasMaxLength(8).IsRequired();
        builder.Property(x => x.Reason).HasMaxLength(500);
        builder.Property(x => x.BlockedBy).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.CnpjBase).IsUnique();
    }
}
