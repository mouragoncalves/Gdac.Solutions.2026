using MediatR;

namespace Gdac.Content.Application.Features.Testimonials.Queries.GetTestimonial;

public record GetTestimonialQuery(Guid Id) : IRequest<TestimonialResult>;

public record TestimonialResult(
    Guid    Id,
    string  AuthorName,
    string? AuthorRole,
    string? AuthorCompany,
    string? AuthorPhotoUrl,
    string  Content,
    int     Rating,
    bool    IsActive,
    int     DisplayOrder,
    Guid?   PartnerId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
