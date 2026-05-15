using MediatR;

namespace Gdac.Content.Application.Features.Testimonials.Commands.DeleteTestimonial;

public record DeleteTestimonialCommand(Guid Id) : IRequest;
