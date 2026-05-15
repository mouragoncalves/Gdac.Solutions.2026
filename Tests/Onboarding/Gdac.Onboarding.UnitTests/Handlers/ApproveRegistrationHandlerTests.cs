using FluentAssertions;
using Gdac.Onboarding.Application.Features.Registrations.Commands.ApproveRegistration;
using Gdac.Onboarding.Domain.Entities;
using Gdac.Onboarding.Domain.Exceptions;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using Gdac.Onboarding.Domain.Interfaces.Services;
using NSubstitute;

namespace Gdac.Onboarding.UnitTests.Handlers;

public class ApproveRegistrationHandlerTests
{
    private readonly IRegistrationRepository _repo = Substitute.For<IRegistrationRepository>();
    private readonly ICoreApiClient _coreApi = Substitute.For<ICoreApiClient>();
    private readonly IAuthApiClient _authApi = Substitute.For<IAuthApiClient>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly ApproveRegistrationHandler _handler;

    private static readonly Guid ReviewedBy = Guid.NewGuid();
    private static readonly Guid CompanyId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    public ApproveRegistrationHandlerTests()
    {
        _uow.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _coreApi.CreateCompanyAsync(Arg.Any<CreateCoreCompanyRequest>(), Arg.Any<CancellationToken>())
            .Returns(CompanyId);
        _authApi.CreateUserAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new CreateAuthUserResult(UserId, "TempPass123!"));
        _coreApi.CreateUserProfileAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _coreApi.LinkUserToCompanyAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _handler = new ApproveRegistrationHandler(_repo, _coreApi, _authApi, _uow);
    }

    [Fact]
    public async Task Handle_RegistrationNotFound_ThrowsNotFoundException()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Registration?)null);

        var command = new ApproveRegistrationCommand(Guid.NewGuid(), ReviewedBy, null);
        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Success_CreatesCompanyAndUserInExternalApis()
    {
        var reg = Registration.CreateClient(
            "João", "joao@empresa.com", null,
            "Empresa LTDA", null, "12345678000195",
            null, null, null, null, null, null, null, null);

        _repo.GetByIdAsync(reg.Id, Arg.Any<CancellationToken>()).Returns(reg);

        var command = new ApproveRegistrationCommand(reg.Id, ReviewedBy, "OK");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.CompanyId.Should().Be(CompanyId);
        result.UserId.Should().Be(UserId);
        result.TemporaryPassword.Should().Be("TempPass123!");

        await _coreApi.Received(1).CreateCompanyAsync(Arg.Any<CreateCoreCompanyRequest>(), Arg.Any<CancellationToken>());
        await _authApi.Received(1).CreateUserAsync("joao@empresa.com", Arg.Any<CancellationToken>());
        await _coreApi.Received(1).CreateUserProfileAsync(UserId, "João", "joao@empresa.com", Arg.Any<CancellationToken>());
        await _coreApi.Received(1).LinkUserToCompanyAsync(CompanyId, UserId, "admin", Arg.Any<CancellationToken>());
        await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Success_SetsRegistrationToApproved()
    {
        var reg = Registration.CreateClient(
            "Maria", "maria@empresa.com", null,
            "Empresa", null, "12345678000195",
            null, null, null, null, null, null, null, null);

        _repo.GetByIdAsync(reg.Id, Arg.Any<CancellationToken>()).Returns(reg);

        await _handler.Handle(new ApproveRegistrationCommand(reg.Id, ReviewedBy, null), CancellationToken.None);

        reg.Status.Should().Be(Gdac.Onboarding.Domain.Enums.RegistrationStatus.Approved);
        reg.ExternalCompanyId.Should().Be(CompanyId);
        reg.ExternalUserId.Should().Be(UserId);
        reg.ReviewedBy.Should().Be(ReviewedBy);
    }
}
