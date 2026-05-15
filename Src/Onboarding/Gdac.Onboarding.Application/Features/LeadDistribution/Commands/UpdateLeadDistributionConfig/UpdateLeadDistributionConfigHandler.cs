using Gdac.Onboarding.Domain.Entities;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Onboarding.Application.Features.LeadDistribution.Commands.UpdateLeadDistributionConfig;

public class UpdateLeadDistributionConfigHandler(
    ILeadDistributionConfigRepository configRepo,
    IUnitOfWork uow)
    : IRequestHandler<UpdateLeadDistributionConfigCommand>
{
    public async Task Handle(UpdateLeadDistributionConfigCommand request, CancellationToken ct)
    {
        var config = await configRepo.GetAsync(ct);

        if (config is null)
        {
            config = LeadDistributionConfig.CreateDefault();
            config.Update(request.Mode, request.DefaultPartnerId, request.UpdatedBy);
            await configRepo.AddAsync(config, ct);
        }
        else
        {
            config.Update(request.Mode, request.DefaultPartnerId, request.UpdatedBy);
            configRepo.Update(config);
        }

        await uow.CommitAsync(ct);
    }
}
