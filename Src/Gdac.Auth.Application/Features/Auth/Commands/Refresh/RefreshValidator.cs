using FluentValidation;

namespace Gdac.Auth.Application.Features.Auth.Commands.Refresh;

public class RefreshValidator : AbstractValidator<RefreshCommand>
{
    public RefreshValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("O token de renovação é obrigatório.");
    }
}
