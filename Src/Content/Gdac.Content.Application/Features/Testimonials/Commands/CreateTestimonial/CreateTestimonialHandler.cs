using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Testimonials.Commands.CreateTestimonial;

public class CreateTestimonialHandler(ITestimonialRepository repo, IUnitOfWork uow)
    : IRequestHandler<CreateTestimonialCommand, Guid>
{
    public async Task<Guid> Handle(CreateTestimonialCommand request, CancellationToken ct)
    {
        var t = Testimonial.Create(
            request.AuthorName, request.AuthorRole, request.AuthorCompany,
            request.AuthorPhotoUrl, request.Content, request.Rating, request.PartnerId);

        await repo.AddAsync(t, ct);
        await uow.CommitAsync(ct);
        return t.Id;
    }
}
