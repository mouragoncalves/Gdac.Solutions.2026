using FluentAssertions;
using Gdac.Core.Application.Features.Companies.Commands.UpdateCompany;
using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Enums;
using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using NSubstitute;

namespace Gdac.Core.UnitTests.Handlers;

public class UpdateCompanyHandlerTests
{
    private readonly ICompanyRepository _repo = Substitute.For<ICompanyRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly UpdateCompanyHandler _handler;

    public UpdateCompanyHandlerTests()
    {
        _uow.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));
        _handler = new UpdateCompanyHandler(_repo, _uow);
    }

    [Fact]
    public async Task Handle_NotFound_ThrowsNotFoundException()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Company?)null);

        var command = new UpdateCompanyCommand(Guid.NewGuid(), "Empresa", null, null, CompanyType.Client, null, null, null, null);
        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_Success_CallsUpdateAndCommit()
    {
        var company = Company.Create("Original", CompanyType.Client);
        _repo.GetByIdAsync(company.Id, Arg.Any<CancellationToken>()).Returns(company);

        var command = new UpdateCompanyCommand(company.Id, "Novo Nome", null, null, CompanyType.Partner, null, null, null, null);
        await _handler.Handle(command, CancellationToken.None);

        company.Name.Should().Be("Novo Nome");
        company.Type.Should().Be(CompanyType.Partner);
        _repo.Received(1).Update(company);
        await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
