using FluentValidation;

namespace Gdac.Auth.Application.Features.Auth.Commands.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("Formato de e-mail inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.");

        RuleFor(x => x.ClientId)
            .NotEmpty().WithMessage("O identificador da aplicação é obrigatório.");

        RuleFor(x => x.ClientSecret)
            .NotEmpty().WithMessage("O secret da aplicação é obrigatório.");
    }
}
