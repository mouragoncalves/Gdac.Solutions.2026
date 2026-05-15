using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Services.Commands.UpdateContentServicePricing;

public class UpdateContentServicePricingHandler(IContentServiceRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateContentServicePricingCommand>
{
    public async Task Handle(UpdateContentServicePricingCommand request, CancellationToken ct)
    {
        var service = await repo.GetByIdWithDetailsAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(ContentService), request.Id);

        service.UpdatePricing(
            request.PrecoRevenda, request.PrecoSugeridoFinal,
            request.DescontoSemestral, request.DescontoAnual,
            request.ChangedBy, request.Notes);

        repo.Update(service);
        await uow.CommitAsync(ct);
    }
}
