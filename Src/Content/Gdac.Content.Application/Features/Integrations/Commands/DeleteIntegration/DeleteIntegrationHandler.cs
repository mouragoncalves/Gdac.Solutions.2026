using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Integrations.Commands.DeleteIntegration;

public class DeleteIntegrationHandler(IIntegrationRepository repo, IUnitOfWork uow)
    : IRequestHandler<DeleteIntegrationCommand>
{
    public async Task Handle(DeleteIntegrationCommand request, CancellationToken ct)
    {
        var integration = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Integration), request.Id);

        integration.SetActive(false);
        repo.Update(integration);
        await uow.CommitAsync(ct);
    }
}
