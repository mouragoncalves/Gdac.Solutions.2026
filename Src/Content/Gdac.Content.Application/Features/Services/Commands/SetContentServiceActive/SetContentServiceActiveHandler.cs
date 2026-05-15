using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Services.Commands.SetContentServiceActive;

public class SetContentServiceActiveHandler(IContentServiceRepository repo, IUnitOfWork uow)
    : IRequestHandler<SetContentServiceActiveCommand>
{
    public async Task Handle(SetContentServiceActiveCommand request, CancellationToken ct)
    {
        var service = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(ContentService), request.Id);

        service.SetActive(request.IsActive);
        repo.Update(service);
        await uow.CommitAsync(ct);
    }
}
