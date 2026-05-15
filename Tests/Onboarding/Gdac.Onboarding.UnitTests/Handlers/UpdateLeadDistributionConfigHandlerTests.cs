using FluentAssertions;
using Gdac.Onboarding.Application.Features.LeadDistribution.Commands.UpdateLeadDistributionConfig;
using Gdac.Onboarding.Domain.Entities;
using Gdac.Onboarding.Domain.Enums;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using NSubstitute;

namespace Gdac.Onboarding.UnitTests.Handlers;

public class UpdateLeadDistributionConfigHandlerTests
{
    private readonly ILeadDistributionConfigRepository _configRepo = Substitute.For<ILeadDistributionConfigRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly UpdateLeadDistributionConfigHandler _handler;

    public UpdateLeadDistributionConfigHandlerTests()
    {
        _uow.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _handler = new UpdateLeadDistributionConfigHandler(_configRepo, _uow);
    }

    [Fact]
    public async Task Handle_NoExistingConfig_CreatesAndSaves()
    {
        _configRepo.GetAsync(Arg.Any<CancellationToken>()).Returns((LeadDistributionConfig?)null);

        var updatedBy = Guid.NewGuid();
        var command = new UpdateLeadDistributionConfigCommand(LeadDistributionMode.Manual, null, updatedBy);
        await _handler.Handle(command, CancellationToken.None);

        await _configRepo.Received(1).AddAsync(
            Arg.Is<LeadDistributionConfig>(c => c.Mode == LeadDistributionMode.Manual && c.UpdatedBy == updatedBy),
            Arg.Any<CancellationToken>());
        await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExistingConfig_UpdatesInPlace()
    {
        var existing = LeadDistributionConfig.CreateDefault();
        _configRepo.GetAsync(Arg.Any<CancellationToken>()).Returns(existing);

        var partnerId = Guid.NewGuid();
        var updatedBy = Guid.NewGuid();
        var command = new UpdateLeadDistributionConfigCommand(LeadDistributionMode.RevendaPadrao, partnerId, updatedBy);
        await _handler.Handle(command, CancellationToken.None);

        existing.Mode.Should().Be(LeadDistributionMode.RevendaPadrao);
        existing.DefaultPartnerId.Should().Be(partnerId);
        existing.UpdatedBy.Should().Be(updatedBy);

        _configRepo.Received(1).Update(existing);
        await _configRepo.DidNotReceive().AddAsync(Arg.Any<LeadDistributionConfig>(), Arg.Any<CancellationToken>());
        await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
