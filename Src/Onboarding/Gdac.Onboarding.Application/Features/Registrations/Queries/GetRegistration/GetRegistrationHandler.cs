using Gdac.Onboarding.Domain.Exceptions;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Onboarding.Application.Features.Registrations.Queries.GetRegistration;

public class GetRegistrationHandler(IRegistrationRepository registrationRepo)
    : IRequestHandler<GetRegistrationQuery, RegistrationResult>
{
    public async Task<RegistrationResult> Handle(GetRegistrationQuery request, CancellationToken ct)
    {
        var r = await registrationRepo.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Cadastro", request.Id);

        return new RegistrationResult(
            r.Id, r.Type, r.Status,
            r.ContactName, r.ContactEmail, r.ContactPhone,
            r.CompanyName, r.TradeName, r.Cnpj,
            r.Segment, r.SizeCategory,
            r.City, r.State,
            r.ReferralCode, r.AssignedPartnerId, r.DistributionMode,
            r.ReviewNotes, r.ReviewedBy, r.ReviewedAt,
            r.ExternalCompanyId, r.ExternalUserId,
            r.IpAddress, r.SubmittedAt, r.UpdatedAt);
    }
}
