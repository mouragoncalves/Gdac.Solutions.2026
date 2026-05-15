using Gdac.Onboarding.Domain.Exceptions;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using Gdac.Onboarding.Domain.Interfaces.Services;
using MediatR;

namespace Gdac.Onboarding.Application.Features.Registrations.Commands.ApproveRegistration;

public class ApproveRegistrationHandler(
    IRegistrationRepository registrationRepo,
    ICoreApiClient coreApi,
    IAuthApiClient authApi,
    IUnitOfWork uow)
    : IRequestHandler<ApproveRegistrationCommand, ApproveRegistrationResult>
{
    public async Task<ApproveRegistrationResult> Handle(ApproveRegistrationCommand request, CancellationToken ct)
    {
        var registration = await registrationRepo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Cadastro", request.Id);

        var companyRequest = new CreateCoreCompanyRequest(
            registration.CompanyName,
            (int)registration.Type,
            registration.TradeName,
            registration.Cnpj,
            registration.ContactEmail,
            registration.ContactPhone,
            (int?)registration.Segment,
            (int?)registration.SizeCategory);

        var coreCompanyId = await coreApi.CreateCompanyAsync(companyRequest, ct);

        var authResult = await authApi.CreateUserAsync(registration.ContactEmail, ct);

        await coreApi.CreateUserProfileAsync(authResult.UserId, registration.ContactName, registration.ContactEmail, ct);
        await coreApi.LinkUserToCompanyAsync(coreCompanyId, authResult.UserId, "admin", ct);

        registration.Approve(request.ReviewedBy, coreCompanyId, authResult.UserId, request.Notes);
        registrationRepo.Update(registration);
        await uow.CommitAsync(ct);

        return new ApproveRegistrationResult(coreCompanyId, authResult.UserId, authResult.TemporaryPassword);
    }
}
