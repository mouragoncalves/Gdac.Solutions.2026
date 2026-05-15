using MediatR;

namespace Gdac.Content.Application.Features.Testimonials.Commands.SetTestimonialActive;

public record SetTestimonialActiveCommand(Guid Id, bool IsActive) : IRequest;
