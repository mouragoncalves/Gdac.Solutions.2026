using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Testimonials.Queries.GetTestimonial;

public class GetTestimonialHandler(ITestimonialRepository repo)
    : IRequestHandler<GetTestimonialQuery, TestimonialResult>
{
    public async Task<TestimonialResult> Handle(GetTestimonialQuery request, CancellationToken ct)
    {
        var t = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Testimonial), request.Id);

        return new TestimonialResult(
            t.Id, t.AuthorName, t.AuthorRole, t.AuthorCompany, t.AuthorPhotoUrl,
            t.Content, t.Rating, t.IsActive, t.DisplayOrder, t.PartnerId,
            t.CreatedAt, t.UpdatedAt);
    }
}
