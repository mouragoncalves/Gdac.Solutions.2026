using FluentAssertions;
using Gdac.Core.Application.Features.BlockList.Commands.BlockCnpj;
using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using NSubstitute;

namespace Gdac.Core.UnitTests.Handlers;

public class BlockCnpjHandlerTests
{
    private readonly IBlockRecordRepository _repo = Substitute.For<IBlockRecordRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly BlockCnpjHandler _handler;

    private static readonly Guid UserId = Guid.NewGuid();

    public BlockCnpjHandlerTests()
    {
        _uow.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));
        _handler = new BlockCnpjHandler(_repo, _uow);
    }

    [Fact]
    public async Task Handle_AlreadyBlocked_ThrowsDomainException()
    {
        _repo.IsBlockedAsync("12345678", Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _handler.Handle(new BlockCnpjCommand("12345678", UserId, null), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*bloqueio*");
    }

    [Fact]
    public async Task Handle_Success_AddsBlockRecord()
    {
        _repo.IsBlockedAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        await _handler.Handle(new BlockCnpjCommand("12345678", UserId, "fraude"), CancellationToken.None);

        await _repo.Received(1).AddAsync(
            Arg.Is<BlockRecord>(r => r.CnpjBase == "12345678" && r.BlockedBy == UserId),
            Arg.Any<CancellationToken>());
        await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
