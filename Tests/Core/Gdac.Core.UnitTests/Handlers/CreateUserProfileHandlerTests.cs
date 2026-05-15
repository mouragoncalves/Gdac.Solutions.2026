using FluentAssertions;
using Gdac.Core.Application.Features.UserProfiles.Commands.CreateUserProfile;
using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using NSubstitute;

namespace Gdac.Core.UnitTests.Handlers;

public class CreateUserProfileHandlerTests
{
    private readonly IUserProfileRepository _repo = Substitute.For<IUserProfileRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly CreateUserProfileHandler _handler;

    public CreateUserProfileHandlerTests()
    {
        _uow.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));
        _handler = new CreateUserProfileHandler(_repo, _uow);
    }

    [Fact]
    public async Task Handle_AlreadyExists_ThrowsDomainException()
    {
        var userId = Guid.NewGuid();
        _repo.ExistsAsync(userId, Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _handler.Handle(new CreateUserProfileCommand(userId, "João", "joao@email.com"), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*Perfil*");
    }

    [Fact]
    public async Task Handle_Success_ReturnsUserId()
    {
        var userId = Guid.NewGuid();
        _repo.ExistsAsync(userId, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _handler.Handle(
            new CreateUserProfileCommand(userId, "João Silva", "joao@email.com"),
            CancellationToken.None);

        result.Should().Be(userId);
        await _repo.Received(1).AddAsync(
            Arg.Is<UserProfile>(p => p.Id == userId && p.FullName == "João Silva"),
            Arg.Any<CancellationToken>());
        await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
