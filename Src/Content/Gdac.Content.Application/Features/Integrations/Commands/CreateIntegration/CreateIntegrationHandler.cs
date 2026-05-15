using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Integrations.Commands.CreateIntegration;

public class CreateIntegrationHandler(IIntegrationRepository repo, IUnitOfWork uow)
    : IRequestHandler<CreateIntegrationCommand, Guid>
{
    public async Task<Guid> Handle(CreateIntegrationCommand request, CancellationToken ct)
    {
        var integration = Integration.Create(
            request.Name, request.Category,
            request.LogoUrl, request.Description, request.ExternalUrl);

        await repo.AddAsync(integration, ct);
        await uow.CommitAsync(ct);
        return integration.Id;
    }
}
