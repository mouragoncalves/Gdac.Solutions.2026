using FluentValidation;

namespace Gdac.Content.Application.Features.Testimonials.Commands.CreateTestimonial;

public class CreateTestimonialValidator : AbstractValidator<CreateTestimonialCommand>
{
    public CreateTestimonialValidator()
    {
        RuleFor(x => x.AuthorName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}
