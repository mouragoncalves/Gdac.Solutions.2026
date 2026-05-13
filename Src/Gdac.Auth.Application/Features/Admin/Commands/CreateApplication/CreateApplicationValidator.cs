using FluentValidation;

namespace Gdac.Auth.Application.Features.Admin.Commands.CreateApplication;

public class CreateApplicationValidator : AbstractValidator<CreateApplicationCommand>
{
    public CreateApplicationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.ClientId).NotEmpty().MaximumLength(100)
            .Matches("^[a-z0-9\\-]+$").WithMessage("O ClientId deve conter apenas letras minúsculas, números e hífens.");
    }
}
