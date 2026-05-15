using Gdac.Content.Application.Features.Testimonials.Queries.GetTestimonial;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Testimonials.Queries.GetTestimonials;

public class GetTestimonialsHandler(ITestimonialRepository repo)
    : IRequestHandler<GetTestimonialsQuery, IReadOnlyList<TestimonialResult>>
{
    public async Task<IReadOnlyList<TestimonialResult>> Handle(GetTestimonialsQuery request, CancellationToken ct)
    {
        var list = await repo.GetActiveAsync(request.PartnerId, ct);
        return list.Select(t => new TestimonialResult(
            t.Id, t.AuthorName, t.AuthorRole, t.AuthorCompany, t.AuthorPhotoUrl,
            t.Content, t.Rating, t.IsActive, t.DisplayOrder, t.PartnerId,
            t.CreatedAt, t.UpdatedAt)).ToList();
    }
}
