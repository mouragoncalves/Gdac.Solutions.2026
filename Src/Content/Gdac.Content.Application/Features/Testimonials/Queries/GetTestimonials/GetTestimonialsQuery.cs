using Gdac.Content.Application.Features.Testimonials.Queries.GetTestimonial;
using MediatR;

namespace Gdac.Content.Application.Features.Testimonials.Queries.GetTestimonials;

public record GetTestimonialsQuery(Guid? PartnerId = null) : IRequest<IReadOnlyList<TestimonialResult>>;
