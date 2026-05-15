using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Services.Commands.UpdateContentServiceInfo;

public class UpdateContentServiceInfoHandler(IContentServiceRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateContentServiceInfoCommand>
{
    public async Task Handle(UpdateContentServiceInfoCommand request, CancellationToken ct)
    {
        var service = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(ContentService), request.Id);

        service.UpdateInfo(request.Name, request.Category, request.Description,
            request.IsFeatured, request.DisplayOrder);

        repo.Update(service);
        await uow.CommitAsync(ct);
    }
}
