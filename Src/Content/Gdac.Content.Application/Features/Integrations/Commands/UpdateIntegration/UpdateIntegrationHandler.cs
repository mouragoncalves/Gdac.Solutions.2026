using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Integrations.Commands.UpdateIntegration;

public class UpdateIntegrationHandler(IIntegrationRepository repo, IUnitOfWork uow)
    : IRequestHandler<UpdateIntegrationCommand>
{
    public async Task Handle(UpdateIntegrationCommand request, CancellationToken ct)
    {
        var integration = await repo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Integration), request.Id);

        integration.Update(request.Name, request.Category, request.LogoUrl, request.Description, request.ExternalUrl);
        repo.Update(integration);
        await uow.CommitAsync(ct);
    }
}
