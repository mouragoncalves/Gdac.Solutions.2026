using FluentAssertions;
using Gdac.Onboarding.Application.Features.Registrations.Commands.SubmitPartnerRegistration;
using Gdac.Onboarding.Domain.Enums;
using Gdac.Onboarding.Domain.Exceptions;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using Gdac.Onboarding.Domain.Interfaces.Services;
using NSubstitute;

namespace Gdac.Onboarding.UnitTests.Handlers;

public class SubmitPartnerRegistrationHandlerTests
{
    private readonly IRegistrationRepository _repo = Substitute.For<IRegistrationRepository>();
    private readonly ICoreApiClient _coreApi = Substitute.For<ICoreApiClient>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly SubmitPartnerRegistrationHandler _handler;

    public SubmitPartnerRegistrationHandlerTests()
    {
        _uow.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _handler = new SubmitPartnerRegistrationHandler(_repo, _coreApi, _uow);
    }

    private static SubmitPartnerRegistrationCommand DefaultCommand() =>
        new("Carlos Parceiro", "carlos@parceiro.com", null,
            "Parceiro LTDA", null, "98765432000110",
            ClientSegment.Tecnologia, null,
            "Curitiba", "PR", "10.0.0.1");

    [Fact]
    public async Task Handle_BlockedCnpj_ThrowsDomainException()
    {
        _coreApi.IsCnpjBlockedAsync("98765432", Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _handler.Handle(DefaultCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*bloqueado*");
    }

    [Fact]
    public async Task Handle_DuplicateCnpj_ThrowsDomainException()
    {
        _coreApi.IsCnpjBlockedAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _repo.ExistsByCnpjAsync("98765432000110", Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _handler.Handle(DefaultCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Handle_Success_CreatesPartnerRegistration()
    {
        _coreApi.IsCnpjBlockedAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _repo.ExistsByCnpjAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        var id = await _handler.Handle(DefaultCommand(), CancellationToken.None);

        id.Should().NotBeEmpty();
        await _repo.Received(1).AddAsync(
            Arg.Is<Gdac.Onboarding.Domain.Entities.Registration>(r => r.Type == RegistrationType.Partner),
            Arg.Any<CancellationToken>());
        await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
