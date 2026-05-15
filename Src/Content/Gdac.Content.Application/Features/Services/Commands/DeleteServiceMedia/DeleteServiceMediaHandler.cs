using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Services.Commands.DeleteServiceMedia;

public class DeleteServiceMediaHandler(IContentServiceRepository repo, IUnitOfWork uow)
    : IRequestHandler<DeleteServiceMediaCommand>
{
    public async Task Handle(DeleteServiceMediaCommand request, CancellationToken ct)
    {
        var service = await repo.GetByIdWithDetailsAsync(request.ServiceId, ct)
            ?? throw new NotFoundException(nameof(ContentService), request.ServiceId);

        var media = service.Media.FirstOrDefault(m => m.Id == request.MediaId)
            ?? throw new NotFoundException(nameof(ServiceMedia), request.MediaId);

        service.Media.Remove(media);
        repo.Update(service);
        await uow.CommitAsync(ct);
    }
}
