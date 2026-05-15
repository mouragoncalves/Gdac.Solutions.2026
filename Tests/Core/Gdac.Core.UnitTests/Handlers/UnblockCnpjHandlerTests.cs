using FluentAssertions;
using Gdac.Core.Application.Features.BlockList.Commands.UnblockCnpj;
using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using NSubstitute;

namespace Gdac.Core.UnitTests.Handlers;

public class UnblockCnpjHandlerTests
{
    private readonly IBlockRecordRepository _repo = Substitute.For<IBlockRecordRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly UnblockCnpjHandler _handler;

    public UnblockCnpjHandlerTests()
    {
        _uow.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));
        _handler = new UnblockCnpjHandler(_repo, _uow);
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsNotFoundException()
    {
        _repo.GetByCnpjBaseAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((BlockRecord?)null);

        var act = () => _handler.Handle(new UnblockCnpjCommand("12345678"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Success_RemovesRecord()
    {
        var record = BlockRecord.Create("12345678", Guid.NewGuid());
        _repo.GetByCnpjBaseAsync("12345678", Arg.Any<CancellationToken>()).Returns(record);

        await _handler.Handle(new UnblockCnpjCommand("12345678"), CancellationToken.None);

        _repo.Received(1).Remove(record);
        await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
