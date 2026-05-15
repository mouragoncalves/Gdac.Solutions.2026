using Gdac.Onboarding.Domain.Entities;
using Gdac.Onboarding.Domain.Exceptions;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using Gdac.Onboarding.Domain.Interfaces.Services;
using MediatR;

namespace Gdac.Onboarding.Application.Features.Registrations.Commands.SubmitPartnerRegistration;

public class SubmitPartnerRegistrationHandler(
    IRegistrationRepository registrationRepo,
    ICoreApiClient coreApi,
    IUnitOfWork uow)
    : IRequestHandler<SubmitPartnerRegistrationCommand, Guid>
{
    public async Task<Guid> Handle(SubmitPartnerRegistrationCommand request, CancellationToken ct)
    {
        var cnpjBase = request.Cnpj.Trim().Substring(0, 8);

        if (await coreApi.IsCnpjBlockedAsync(cnpjBase, ct))
            throw new DomainException("Este CNPJ está bloqueado e não pode realizar cadastro.");

        if (await registrationRepo.ExistsByCnpjAsync(request.Cnpj.Trim(), ct))
            throw new DomainException("Já existe um cadastro com este CNPJ.");

        var registration = Registration.CreatePartner(
            request.ContactName, request.ContactEmail, request.ContactPhone,
            request.CompanyName, request.TradeName, request.Cnpj,
            request.Segment, request.SizeCategory,
            request.City, request.State, request.IpAddress);

        await registrationRepo.AddAsync(registration, ct);
        await uow.CommitAsync(ct);
        return registration.Id;
    }
}
