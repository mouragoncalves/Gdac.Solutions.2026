using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Services.Commands.AddServiceMedia;

public class AddServiceMediaHandler(IContentServiceRepository repo, IUnitOfWork uow)
    : IRequestHandler<AddServiceMediaCommand, Guid>
{
    public async Task<Guid> Handle(AddServiceMediaCommand request, CancellationToken ct)
    {
        var service = await repo.GetByIdWithDetailsAsync(request.ServiceId, ct)
            ?? throw new NotFoundException(nameof(ContentService), request.ServiceId);

        var media = ServiceMedia.Create(request.ServiceId, request.Url, request.Type, request.DisplayOrder);
        service.Media.Add(media);

        repo.Update(service);
        await uow.CommitAsync(ct);
        return media.Id;
    }
}
