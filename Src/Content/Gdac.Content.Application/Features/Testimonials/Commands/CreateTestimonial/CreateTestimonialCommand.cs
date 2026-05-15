using MediatR;

namespace Gdac.Content.Application.Features.Testimonials.Commands.CreateTestimonial;

public record CreateTestimonialCommand(
    string  AuthorName,
    string? AuthorRole,
    string? AuthorCompany,
    string? AuthorPhotoUrl,
    string  Content,
    int     Rating,
    Guid?   PartnerId) : IRequest<Guid>;
