using FluentAssertions;
using Gdac.Onboarding.Domain.Entities;
using Gdac.Onboarding.Domain.Enums;

namespace Gdac.Onboarding.UnitTests.Domain;

public class RegistrationTests
{
    [Fact]
    public void CreateClient_SetsCorrectInitialState()
    {
        var reg = Registration.CreateClient(
            "João Silva", "joao@empresa.com.br", "11999999999",
            "Empresa LTDA", null, "12345678000195",
            ClientSegment.Servicos, CompanySize.Pequena,
            "São Paulo", "SP", null,
            null, null, "192.168.1.1");

        reg.Id.Should().NotBeEmpty();
        reg.Type.Should().Be(RegistrationType.Client);
        reg.Status.Should().Be(RegistrationStatus.Pending);
        reg.ContactEmail.Should().Be("joao@empresa.com.br");
        reg.Cnpj.Should().Be("12345678000195");
        reg.CnpjBase.Should().Be("12345678");
        reg.State.Should().Be("SP");
    }

    [Fact]
    public void CreateClient_NormalizesEmailToLowercase()
    {
        var reg = Registration.CreateClient(
            "Maria", "MARIA@EMPRESA.COM", null,
            "Empresa", null, "12345678000195",
            null, null, null, null, null, null, null, null);

        reg.ContactEmail.Should().Be("maria@empresa.com");
    }

    [Fact]
    public void CreateClient_ExtractsCnpjBaseFromFirst8Digits()
    {
        var reg = Registration.CreateClient(
            "Nome", "email@test.com", null,
            "Empresa", null, "98765432000110",
            null, null, null, null, null, null, null, null);

        reg.CnpjBase.Should().Be("98765432");
    }

    [Fact]
    public void CreatePartner_SetsCorrectTypeAndStatus()
    {
        var reg = Registration.CreatePartner(
            "Carlos", "carlos@parceiro.com", null,
            "Parceiro LTDA", null, "12345678000195",
            null, null, "Curitiba", "PR", null);

        reg.Type.Should().Be(RegistrationType.Partner);
        reg.Status.Should().Be(RegistrationStatus.Pending);
        reg.ReferralCode.Should().BeNull();
        reg.AssignedPartnerId.Should().BeNull();
    }

    [Fact]
    public void Approve_SetsApprovedStatusAndExternalIds()
    {
        var reg = Registration.CreateClient(
            "Nome", "email@test.com", null,
            "Empresa", null, "12345678000195",
            null, null, null, null, null, null, null, null);

        var reviewedBy = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        reg.Approve(reviewedBy, companyId, userId, "Aprovado");

        reg.Status.Should().Be(RegistrationStatus.Approved);
        reg.ReviewedBy.Should().Be(reviewedBy);
        reg.ExternalCompanyId.Should().Be(companyId);
        reg.ExternalUserId.Should().Be(userId);
        reg.ReviewNotes.Should().Be("Aprovado");
        reg.ReviewedAt.Should().NotBeNull();
    }

    [Fact]
    public void Reject_SetsRejectedStatus()
    {
        var reg = Registration.CreateClient(
            "Nome", "email@test.com", null,
            "Empresa", null, "12345678000195",
            null, null, null, null, null, null, null, null);

        var reviewedBy = Guid.NewGuid();
        reg.Reject(reviewedBy, "Documentação incompleta");

        reg.Status.Should().Be(RegistrationStatus.Rejected);
        reg.ReviewedBy.Should().Be(reviewedBy);
        reg.ReviewNotes.Should().Be("Documentação incompleta");
        reg.ExternalCompanyId.Should().BeNull();
        reg.ExternalUserId.Should().BeNull();
    }

    [Fact]
    public void AssignPartner_SetsPartnerAndMode()
    {
        var reg = Registration.CreateClient(
            "Nome", "email@test.com", null,
            "Empresa", null, "12345678000195",
            null, null, null, null, null, null, null, null);

        var partnerId = Guid.NewGuid();
        reg.AssignPartner(partnerId, LeadDistributionMode.RevendaPadrao);

        reg.AssignedPartnerId.Should().Be(partnerId);
        reg.DistributionMode.Should().Be(LeadDistributionMode.RevendaPadrao);
    }
}
