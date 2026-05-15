using FluentAssertions;
using Gdac.Core.Domain.Entities;
using Gdac.Core.Domain.Enums;

namespace Gdac.Core.UnitTests.Domain;

public class CompanyTests
{
    [Fact]
    public void Create_SetsProspectStatus()
    {
        var company = Company.Create("Empresa Teste", CompanyType.Client);

        company.Status.Should().Be(CompanyStatus.Prospect);
    }

    [Fact]
    public void Create_TrimsName()
    {
        var company = Company.Create("  Empresa  ", CompanyType.Client);

        company.Name.Should().Be("Empresa");
    }

    [Fact]
    public void Create_LowercasesEmail()
    {
        var company = Company.Create("Empresa", CompanyType.Client, email: "CONTATO@EMPRESA.COM");

        company.Email.Should().Be("contato@empresa.com");
    }

    [Fact]
    public void Create_GeneratesNewId()
    {
        var a = Company.Create("Empresa A", CompanyType.Client);
        var b = Company.Create("Empresa B", CompanyType.Client);

        a.Id.Should().NotBe(b.Id);
        a.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void SetStatus_ChangesStatus()
    {
        var company = Company.Create("Empresa", CompanyType.Client);

        company.SetStatus(CompanyStatus.Active);

        company.Status.Should().Be(CompanyStatus.Active);
    }

    [Fact]
    public void Update_ChangesNameAndEmail()
    {
        var company = Company.Create("Original", CompanyType.Client, email: "old@email.com");

        company.Update("Novo Nome", null, null, CompanyType.Partner, "NOVO@EMAIL.COM", null, null, null);

        company.Name.Should().Be("Novo Nome");
        company.Email.Should().Be("novo@email.com");
    }
}
