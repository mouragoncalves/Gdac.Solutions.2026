using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Testimonials.Commands.DeleteTestimonial;

public class DeleteTestimonialHandler(ITestimonialRepository repo, IUnitOfWork uow)
    : IRequestHandler<DeleteTestimonialCommand>
{
    public async Task Handle(DeleteTestimonialCommand request, CancellationToken ct)
    {
        var t = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Testimonial), request.Id);

        t.SetActive(false);
        repo.Update(t);
        await uow.CommitAsync(ct);
    }
}
