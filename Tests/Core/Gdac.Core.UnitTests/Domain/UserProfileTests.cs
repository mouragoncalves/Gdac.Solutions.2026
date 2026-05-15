using FluentAssertions;
using Gdac.Core.Domain.Entities;

namespace Gdac.Core.UnitTests.Domain;

public class UserProfileTests
{
    [Fact]
    public void Create_UsesExternalId()
    {
        var id = Guid.NewGuid();
        var profile = UserProfile.Create(id, "João Silva", "joao@email.com");

        profile.Id.Should().Be(id);
    }

    [Fact]
    public void Create_LowercasesEmail()
    {
        var profile = UserProfile.Create(Guid.NewGuid(), "João", "JOAO@EMAIL.COM");

        profile.Email.Should().Be("joao@email.com");
    }

    [Fact]
    public void Create_IsActiveByDefault()
    {
        var profile = UserProfile.Create(Guid.NewGuid(), "João", "joao@email.com");

        profile.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_TrimsFullName()
    {
        var profile = UserProfile.Create(Guid.NewGuid(), "  João Silva  ", "joao@email.com");

        profile.FullName.Should().Be("João Silva");
    }

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var profile = UserProfile.Create(Guid.NewGuid(), "João", "joao@email.com");

        profile.Deactivate();

        profile.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_SetsIsActiveTrue()
    {
        var profile = UserProfile.Create(Guid.NewGuid(), "João", "joao@email.com");
        profile.Deactivate();

        profile.Activate();

        profile.IsActive.Should().BeTrue();
    }
}
