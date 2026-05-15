using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Testimonials.Commands.SetTestimonialActive;

public class SetTestimonialActiveHandler(ITestimonialRepository repo, IUnitOfWork uow)
    : IRequestHandler<SetTestimonialActiveCommand>
{
    public async Task Handle(SetTestimonialActiveCommand request, CancellationToken ct)
    {
        var t = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Testimonial), request.Id);

        t.SetActive(request.IsActive);
        repo.Update(t);
        await uow.CommitAsync(ct);
    }
}
