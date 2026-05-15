using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Services.Commands.DeleteContentService;

public class DeleteContentServiceHandler(IContentServiceRepository repo, IUnitOfWork uow)
    : IRequestHandler<DeleteContentServiceCommand>
{
    public async Task Handle(DeleteContentServiceCommand request, CancellationToken ct)
    {
        var service = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(ContentService), request.Id);

        service.SetActive(false);
        repo.Update(service);
        await uow.CommitAsync(ct);
    }
}
