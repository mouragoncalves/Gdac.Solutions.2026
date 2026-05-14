using FluentValidation;

namespace Gdac.Core.Application.Features.UserProfiles.Commands.UpdateUserProfile;

public class UpdateUserProfileValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone != null);
        RuleFor(x => x.Cpf).Length(11).When(x => x.Cpf != null);
    }
}
