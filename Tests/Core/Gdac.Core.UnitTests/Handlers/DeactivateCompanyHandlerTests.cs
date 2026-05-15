using FluentAssertions;
using Gdac.Core.Application.Features.Companies.Commands.DeactivateCompany;
using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Enums;
using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using NSubstitute;

namespace Gdac.Core.UnitTests.Handlers;

public class DeactivateCompanyHandlerTests
{
    private readonly ICompanyRepository _repo = Substitute.For<ICompanyRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly DeactivateCompanyHandler _handler;

    public DeactivateCompanyHandlerTests()
    {
        _uow.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));
        _handler = new DeactivateCompanyHandler(_repo, _uow);
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsNotFoundException()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Company?)null);

        var act = () => _handler.Handle(new DeactivateCompanyCommand(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Success_SetsStatusInactive()
    {
        var company = Company.Create("Empresa", CompanyType.Client);
        _repo.GetByIdAsync(company.Id, Arg.Any<CancellationToken>()).Returns(company);

        await _handler.Handle(new DeactivateCompanyCommand(company.Id), CancellationToken.None);

        company.Status.Should().Be(CompanyStatus.Inactive);
        _repo.Received(1).Update(company);
        await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
