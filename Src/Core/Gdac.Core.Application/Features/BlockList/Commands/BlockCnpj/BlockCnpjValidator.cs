using FluentValidation;

namespace Gdac.Core.Application.Features.BlockList.Commands.BlockCnpj;

public class BlockCnpjValidator : AbstractValidator<BlockCnpjCommand>
{
    public BlockCnpjValidator()
    {
        RuleFor(x => x.CnpjBase)
            .NotEmpty()
            .Length(8).WithMessage("CnpjBase deve ter exatamente 8 dígitos.")
            .Matches(@"^\d{8}$").WithMessage("CnpjBase deve conter apenas dígitos.");

        RuleFor(x => x.BlockedBy).NotEmpty();
    }
}
