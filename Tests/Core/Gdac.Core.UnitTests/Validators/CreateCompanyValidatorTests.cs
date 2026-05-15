using FluentAssertions;
using FluentValidation.TestHelper;
using Gdac.Core.Application.Features.Companies.Commands.CreateCompany;
using Gdac.Core.Domain.Enums;

namespace Gdac.Core.UnitTests.Validators;

public class CreateCompanyValidatorTests
{
    private readonly CreateCompanyValidator _validator = new();

    private static CreateCompanyCommand Valid() =>
        new("Empresa Teste", CompanyType.Client, null, null, null, null, null, null);

    [Fact]
    public void Validate_EmptyName_IsInvalid()
    {
        var result = _validator.TestValidate(Valid() with { Name = "" });
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NameTooLong_IsInvalid()
    {
        var result = _validator.TestValidate(Valid() with { Name = new string('A', 201) });
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_InvalidType_IsInvalid()
    {
        var result = _validator.TestValidate(Valid() with { Type = (CompanyType)99 });
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Validate_CnpjNotLength14_IsInvalid()
    {
        var result = _validator.TestValidate(Valid() with { Cnpj = "123" });
        result.ShouldHaveValidationErrorFor(x => x.Cnpj);
    }

    [Fact]
    public void Validate_InvalidEmail_IsInvalid()
    {
        var result = _validator.TestValidate(Valid() with { Email = "not-an-email" });
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_ValidRequest_IsValid()
    {
        var command = new CreateCompanyCommand(
            "Empresa Teste", CompanyType.Client, "Nome Fantasia",
            "12345678000195", "contato@empresa.com", null, null, null);

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_NullOptionalFields_IsValid()
    {
        var result = _validator.TestValidate(Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }
}
