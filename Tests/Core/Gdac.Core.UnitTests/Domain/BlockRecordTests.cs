using FluentAssertions;
using Gdac.Core.Domain.Entities;

namespace Gdac.Core.UnitTests.Domain;

public class BlockRecordTests
{
    [Fact]
    public void Create_TrimsCnpjBase()
    {
        var record = BlockRecord.Create("  12345678  ", Guid.NewGuid());

        record.CnpjBase.Should().Be("12345678");
    }

    [Fact]
    public void Create_SetsBlockedBy()
    {
        var userId = Guid.NewGuid();
        var record = BlockRecord.Create("12345678", userId, "fraude");

        record.BlockedBy.Should().Be(userId);
        record.Reason.Should().Be("fraude");
    }

    [Fact]
    public void Create_GeneratesNewId()
    {
        var a = BlockRecord.Create("12345678", Guid.NewGuid());
        var b = BlockRecord.Create("98765432", Guid.NewGuid());

        a.Id.Should().NotBe(b.Id);
        a.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_NullReason_IsNull()
    {
        var record = BlockRecord.Create("12345678", Guid.NewGuid(), null);

        record.Reason.Should().BeNull();
    }
}
