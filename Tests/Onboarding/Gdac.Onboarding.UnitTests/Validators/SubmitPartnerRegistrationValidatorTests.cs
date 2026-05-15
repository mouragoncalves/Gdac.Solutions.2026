using FluentValidation.TestHelper;
using Gdac.Onboarding.Application.Features.Registrations.Commands.SubmitPartnerRegistration;

namespace Gdac.Onboarding.UnitTests.Validators;

public class SubmitPartnerRegistrationValidatorTests
{
    private readonly SubmitPartnerRegistrationValidator _validator = new();

    private static SubmitPartnerRegistrationCommand Valid() =>
        new("Carlos Parceiro", "carlos@parceiro.com", null,
            "Parceiro LTDA", null, "98765432000110",
            null, null, "Curitiba", "PR", null);

    [Fact]
    public void Valid_Command_PassesValidation()
    {
        var result = _validator.TestValidate(Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyCompanyName_FailsValidation()
    {
        var result = _validator.TestValidate(Valid() with { CompanyName = "" });
        result.ShouldHaveValidationErrorFor(x => x.CompanyName);
    }

    [Theory]
    [InlineData("1234567800019")]    // 13 dígitos
    [InlineData("123456780001955")] // 15 dígitos
    public void CnpjWithWrongLength_FailsValidation(string cnpj)
    {
        var result = _validator.TestValidate(Valid() with { Cnpj = cnpj });
        result.ShouldHaveValidationErrorFor(x => x.Cnpj);
    }
}
