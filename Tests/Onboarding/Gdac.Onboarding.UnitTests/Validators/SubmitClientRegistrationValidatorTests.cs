using FluentValidation.TestHelper;
using Gdac.Onboarding.Application.Features.Registrations.Commands.SubmitClientRegistration;
using Gdac.Onboarding.Domain.Enums;

namespace Gdac.Onboarding.UnitTests.Validators;

public class SubmitClientRegistrationValidatorTests
{
    private readonly SubmitClientRegistrationValidator _validator = new();

    private static SubmitClientRegistrationCommand Valid() =>
        new("João Silva", "joao@empresa.com", null,
            "Empresa LTDA", null, "12345678000195",
            null, null, null, null, null, null);

    [Fact]
    public void Valid_Command_PassesValidation()
    {
        var result = _validator.TestValidate(Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyContactName_FailsValidation(string name)
    {
        var result = _validator.TestValidate(Valid() with { ContactName = name });
        result.ShouldHaveValidationErrorFor(x => x.ContactName);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("@semdominio")]
    [InlineData("")]
    public void InvalidEmail_FailsValidation(string email)
    {
        var result = _validator.TestValidate(Valid() with { ContactEmail = email });
        result.ShouldHaveValidationErrorFor(x => x.ContactEmail);
    }

    [Theory]
    [InlineData("1234567")]           // 7 dígitos
    [InlineData("123456789012345")]   // 15 dígitos
    [InlineData("1234567800019A")]    // letra
    public void InvalidCnpj_FailsValidation(string cnpj)
    {
        var result = _validator.TestValidate(Valid() with { Cnpj = cnpj });
        result.ShouldHaveValidationErrorFor(x => x.Cnpj);
    }

    [Fact]
    public void CnpjWith14Digits_PassesValidation()
    {
        var result = _validator.TestValidate(Valid() with { Cnpj = "12345678000195" });
        result.ShouldNotHaveValidationErrorFor(x => x.Cnpj);
    }

    [Fact]
    public void StateLongerThan2_FailsValidation()
    {
        var result = _validator.TestValidate(Valid() with { State = "SPX" });
        result.ShouldHaveValidationErrorFor(x => x.State);
    }

    [Fact]
    public void InvalidSegmentValue_FailsValidation()
    {
        var result = _validator.TestValidate(Valid() with { Segment = (ClientSegment)999 });
        result.ShouldHaveValidationErrorFor(x => x.Segment);
    }
}
