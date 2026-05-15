using Gdac.Onboarding.Domain.Entities;
using Gdac.Onboarding.Domain.Enums;
using Gdac.Onboarding.Domain.Exceptions;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using Gdac.Onboarding.Domain.Interfaces.Services;
using MediatR;

namespace Gdac.Onboarding.Application.Features.Registrations.Commands.SubmitClientRegistration;

public class SubmitClientRegistrationHandler(
    IRegistrationRepository registrationRepo,
    ILeadDistributionConfigRepository configRepo,
    ICoreApiClient coreApi,
    IUnitOfWork uow)
    : IRequestHandler<SubmitClientRegistrationCommand, Guid>
{
    public async Task<Guid> Handle(SubmitClientRegistrationCommand request, CancellationToken ct)
    {
        var cnpjBase = request.Cnpj.Trim().Substring(0, 8);

        if (await coreApi.IsCnpjBlockedAsync(cnpjBase, ct))
            throw new DomainException("Este CNPJ está bloqueado e não pode realizar cadastro.");

        if (await registrationRepo.ExistsByCnpjAsync(request.Cnpj.Trim(), ct))
            throw new DomainException("Já existe um cadastro com este CNPJ.");

        Guid? assignedPartnerId = null;
        LeadDistributionMode? distributionMode = null;

        if (!string.IsNullOrWhiteSpace(request.ReferralCode))
        {
            distributionMode = LeadDistributionMode.Manual;
        }
        else
        {
            var config = await configRepo.GetAsync(ct);
            if (config is not null && config.Mode == LeadDistributionMode.RevendaPadrao && config.DefaultPartnerId.HasValue)
            {
                assignedPartnerId = config.DefaultPartnerId;
                distributionMode  = LeadDistributionMode.RevendaPadrao;
            }
            else if (config is not null)
            {
                distributionMode = config.Mode;
            }
        }

        var registration = Registration.CreateClient(
            request.ContactName, request.ContactEmail, request.ContactPhone,
            request.CompanyName, request.TradeName, request.Cnpj,
            request.Segment, request.SizeCategory,
            request.City, request.State, request.ReferralCode,
            assignedPartnerId, distributionMode,
            request.IpAddress);

        await registrationRepo.AddAsync(registration, ct);
        await uow.CommitAsync(ct);
        return registration.Id;
    }
}
