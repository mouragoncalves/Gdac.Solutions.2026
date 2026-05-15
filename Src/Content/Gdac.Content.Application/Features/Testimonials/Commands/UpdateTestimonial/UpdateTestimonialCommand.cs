using MediatR;

namespace Gdac.Content.Application.Features.Testimonials.Commands.UpdateTestimonial;

public record UpdateTestimonialCommand(
    Guid    Id,
    string  AuthorName,
    string? AuthorRole,
    string? AuthorCompany,
    string? AuthorPhotoUrl,
    string  Content,
    int     Rating) : IRequest;
