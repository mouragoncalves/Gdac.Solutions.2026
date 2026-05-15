using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Integrations.Commands.SetIntegrationActive;

public class SetIntegrationActiveHandler(IIntegrationRepository repo, IUnitOfWork uow)
    : IRequestHandler<SetIntegrationActiveCommand>
{
    public async Task Handle(SetIntegrationActiveCommand request, CancellationToken ct)
    {
        var integration = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Integration), request.Id);

        integration.SetActive(request.IsActive);
        repo.Update(integration);
        await uow.CommitAsync(ct);
    }
}
