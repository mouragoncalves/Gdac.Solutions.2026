using FluentValidation;

namespace Gdac.Auth.Application.Features.Demo.Commands.RegisterDemo;

public class RegisterDemoValidator : AbstractValidator<RegisterDemoCommand>
{
    public RegisterDemoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(200).WithMessage("O nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("Formato de e-mail inválido.");
    }
}
