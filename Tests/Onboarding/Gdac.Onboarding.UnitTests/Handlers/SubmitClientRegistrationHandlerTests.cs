using FluentAssertions;
using Gdac.Onboarding.Application.Features.Registrations.Commands.SubmitClientRegistration;
using Gdac.Onboarding.Domain.Entities;
using Gdac.Onboarding.Domain.Enums;
using Gdac.Onboarding.Domain.Exceptions;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using Gdac.Onboarding.Domain.Interfaces.Services;
using NSubstitute;

namespace Gdac.Onboarding.UnitTests.Handlers;

public class SubmitClientRegistrationHandlerTests
{
    private readonly IRegistrationRepository _repo = Substitute.For<IRegistrationRepository>();
    private readonly ILeadDistributionConfigRepository _configRepo = Substitute.For<ILeadDistributionConfigRepository>();
    private readonly ICoreApiClient _coreApi = Substitute.For<ICoreApiClient>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly SubmitClientRegistrationHandler _handler;

    public SubmitClientRegistrationHandlerTests()
    {
        _uow.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _handler = new SubmitClientRegistrationHandler(_repo, _configRepo, _coreApi, _uow);
    }

    private static SubmitClientRegistrationCommand DefaultCommand(string? referralCode = null) =>
        new("João Silva", "joao@empresa.com", "11999999999",
            "Empresa LTDA", null, "12345678000195",
            ClientSegment.Servicos, null,
            "São Paulo", "SP", referralCode, "127.0.0.1");

    [Fact]
    public async Task Handle_BlockedCnpj_ThrowsDomainException()
    {
        _coreApi.IsCnpjBlockedAsync("12345678", Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _handler.Handle(DefaultCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*bloqueado*");
    }

    [Fact]
    public async Task Handle_DuplicateCnpj_ThrowsDomainException()
    {
        _coreApi.IsCnpjBlockedAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _repo.ExistsByCnpjAsync("12345678000195", Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _handler.Handle(DefaultCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*cadastro*");
    }

    [Fact]
    public async Task Handle_WithReferralCode_SetsManualDistributionMode()
    {
        _coreApi.IsCnpjBlockedAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _repo.ExistsByCnpjAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        Registration? savedReg = null;
        await _repo.AddAsync(Arg.Do<Registration>(r => savedReg = r), Arg.Any<CancellationToken>());

        var id = await _handler.Handle(DefaultCommand(referralCode: "PARCEIRO01"), CancellationToken.None);

        id.Should().NotBeEmpty();
        savedReg.Should().NotBeNull();
        savedReg!.DistributionMode.Should().Be(LeadDistributionMode.Manual);
        savedReg.AssignedPartnerId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_RevendaPadraoConfig_AssignsDefaultPartner()
    {
        var defaultPartnerId = Guid.NewGuid();
        var config = LeadDistributionConfig.CreateDefault();
        config.Update(LeadDistributionMode.RevendaPadrao, defaultPartnerId, Guid.Empty);

        _coreApi.IsCnpjBlockedAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _repo.ExistsByCnpjAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _configRepo.GetAsync(Arg.Any<CancellationToken>()).Returns(config);

        Registration? savedReg = null;
        await _repo.AddAsync(Arg.Do<Registration>(r => savedReg = r), Arg.Any<CancellationToken>());

        await _handler.Handle(DefaultCommand(), CancellationToken.None);

        savedReg!.AssignedPartnerId.Should().Be(defaultPartnerId);
        savedReg.DistributionMode.Should().Be(LeadDistributionMode.RevendaPadrao);
    }

    [Fact]
    public async Task Handle_Success_CommitsAndReturnsId()
    {
        _coreApi.IsCnpjBlockedAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _repo.ExistsByCnpjAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _configRepo.GetAsync(Arg.Any<CancellationToken>()).Returns((LeadDistributionConfig?)null);

        var id = await _handler.Handle(DefaultCommand(), CancellationToken.None);

        id.Should().NotBeEmpty();
        await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
