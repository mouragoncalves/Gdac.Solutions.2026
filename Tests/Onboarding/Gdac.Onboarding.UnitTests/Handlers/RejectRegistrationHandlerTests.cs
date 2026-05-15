using FluentAssertions;
using Gdac.Onboarding.Application.Features.Registrations.Commands.RejectRegistration;
using Gdac.Onboarding.Domain.Entities;
using Gdac.Onboarding.Domain.Enums;
using Gdac.Onboarding.Domain.Exceptions;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using NSubstitute;

namespace Gdac.Onboarding.UnitTests.Handlers;

public class RejectRegistrationHandlerTests
{
    private readonly IRegistrationRepository _repo = Substitute.For<IRegistrationRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly RejectRegistrationHandler _handler;

    public RejectRegistrationHandlerTests()
    {
        _uow.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _handler = new RejectRegistrationHandler(_repo, _uow);
    }

    [Fact]
    public async Task Handle_RegistrationNotFound_ThrowsNotFoundException()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Registration?)null);

        var act = () => _handler.Handle(
            new RejectRegistrationCommand(Guid.NewGuid(), Guid.NewGuid(), null),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Success_SetsRegistrationToRejected()
    {
        var reg = Registration.CreateClient(
            "Nome", "email@test.com", null,
            "Empresa", null, "12345678000195",
            null, null, null, null, null, null, null, null);

        _repo.GetByIdAsync(reg.Id, Arg.Any<CancellationToken>()).Returns(reg);

        var reviewedBy = Guid.NewGuid();
        await _handler.Handle(
            new RejectRegistrationCommand(reg.Id, reviewedBy, "Dados inconsistentes"),
            CancellationToken.None);

        reg.Status.Should().Be(RegistrationStatus.Rejected);
        reg.ReviewedBy.Should().Be(reviewedBy);
        reg.ReviewNotes.Should().Be("Dados inconsistentes");
        await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
