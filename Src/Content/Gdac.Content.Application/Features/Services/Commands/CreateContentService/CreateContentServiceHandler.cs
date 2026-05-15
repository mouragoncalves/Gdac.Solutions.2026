using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Services.Commands.CreateContentService;

public class CreateContentServiceHandler(IContentServiceRepository repo, IUnitOfWork uow)
    : IRequestHandler<CreateContentServiceCommand, Guid>
{
    public async Task<Guid> Handle(CreateContentServiceCommand request, CancellationToken ct)
    {
        var service = ContentService.Create(
            request.Name, request.Category, request.Description,
            request.PrecoRevenda, request.PrecoSugeridoFinal,
            request.DescontoSemestral, request.DescontoAnual);

        await repo.AddAsync(service, ct);
        await uow.CommitAsync(ct);
        return service.Id;
    }
}
