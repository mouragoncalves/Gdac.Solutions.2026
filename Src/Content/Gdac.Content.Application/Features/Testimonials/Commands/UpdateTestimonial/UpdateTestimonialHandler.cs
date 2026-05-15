using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Testimonials.Commands.UpdateTestimonial;

public class UpdateTestimonialHandler(ITestimonialRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateTestimonialCommand>
{
    public async Task Handle(UpdateTestimonialCommand request, CancellationToken ct)
    {
        var t = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Testimonial), request.Id);

        t.Update(request.AuthorName, request.AuthorRole, request.AuthorCompany,
            request.AuthorPhotoUrl, request.Content, request.Rating);

        repo.Update(t);
        await uow.CommitAsync(ct);
    }
}
