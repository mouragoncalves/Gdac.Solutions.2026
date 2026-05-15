using FluentAssertions;
using FluentValidation.TestHelper;
using Gdac.Core.Application.Features.BlockList.Commands.BlockCnpj;

namespace Gdac.Core.UnitTests.Validators;

public class BlockCnpjValidatorTests
{
    private readonly BlockCnpjValidator _validator = new();

    private static BlockCnpjCommand Valid() =>
        new("12345678", Guid.NewGuid(), null);

    [Fact]
    public void Validate_EmptyCnpjBase_IsInvalid()
    {
        var result = _validator.TestValidate(Valid() with { CnpjBase = "" });
        result.ShouldHaveValidationErrorFor(x => x.CnpjBase);
    }

    [Fact]
    public void Validate_CnpjBaseNot8Digits_IsInvalid()
    {
        var result = _validator.TestValidate(Valid() with { CnpjBase = "1234" });
        result.ShouldHaveValidationErrorFor(x => x.CnpjBase);
    }

    [Fact]
    public void Validate_CnpjBaseWithLetters_IsInvalid()
    {
        var result = _validator.TestValidate(Valid() with { CnpjBase = "1234567A" });
        result.ShouldHaveValidationErrorFor(x => x.CnpjBase);
    }

    [Fact]
    public void Validate_EmptyBlockedBy_IsInvalid()
    {
        var result = _validator.TestValidate(Valid() with { BlockedBy = Guid.Empty });
        result.ShouldHaveValidationErrorFor(x => x.BlockedBy);
    }

    [Fact]
    public void Validate_ValidCommand_IsValid()
    {
        var result = _validator.TestValidate(Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }
}
