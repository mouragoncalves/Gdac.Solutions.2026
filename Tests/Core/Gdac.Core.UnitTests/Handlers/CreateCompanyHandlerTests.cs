using FluentAssertions;
using Gdac.Core.Application.Features.Companies.Commands.CreateCompany;
using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Enums;
using Gdac.Core.Domain.Exceptions;
using Gdac.Core.Domain.Interfaces.Repositories;
using NSubstitute;

namespace Gdac.Core.UnitTests.Handlers;

public class CreateCompanyHandlerTests
{
    private readonly ICompanyRepository _repo = Substitute.For<ICompanyRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly CreateCompanyHandler _handler;

    public CreateCompanyHandlerTests()
    {
        _uow.CommitAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1));
        _handler = new CreateCompanyHandler(_repo, _uow);
    }

    [Fact]
    public async Task Handle_DuplicateCnpj_ThrowsDomainException()
    {
        _repo.ExistsByCnpjAsync("12345678000195", Arg.Any<CancellationToken>()).Returns(true);

        var command = new CreateCompanyCommand("Empresa", CompanyType.Client, null, "12345678000195", null, null, null, null);
        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*CNPJ*");
    }

    [Fact]
    public async Task Handle_NullCnpj_SkipsDuplicateCheck()
    {
        var command = new CreateCompanyCommand("Empresa", CompanyType.Client, null, null, null, null, null, null);

        var id = await _handler.Handle(command, CancellationToken.None);

        id.Should().NotBeEmpty();
        await _repo.DidNotReceive().ExistsByCnpjAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Success_ReturnsNewId()
    {
        _repo.ExistsByCnpjAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        var command = new CreateCompanyCommand("Empresa", CompanyType.Client, null, "12345678000195", null, null, null, null);
        var id = await _handler.Handle(command, CancellationToken.None);

        id.Should().NotBeEmpty();
        await _repo.Received(1).AddAsync(Arg.Any<Company>(), Arg.Any<CancellationToken>());
        await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}
