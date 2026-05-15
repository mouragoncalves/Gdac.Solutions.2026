using Gdac.Onboarding.Application.Features.Registrations.Queries.GetRegistration;
using Gdac.Onboarding.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Onboarding.Application.Features.Registrations.Queries.GetRegistrations;

public class GetRegistrationsHandler(IRegistrationRepository registrationRepo)
    : IRequestHandler<GetRegistrationsQuery, IReadOnlyList<RegistrationResult>>
{
    public async Task<IReadOnlyList<RegistrationResult>> Handle(GetRegistrationsQuery request, CancellationToken ct)
    {
        var registrations = request.Status.HasValue
            ? await registrationRepo.GetByStatusAsync(request.Status.Value, ct)
            : await registrationRepo.GetAllAsync(ct);

        return registrations.Select(r => new RegistrationResult(
            r.Id, r.Type, r.Status,
            r.ContactName, r.ContactEmail, r.ContactPhone,
            r.CompanyName, r.TradeName, r.Cnpj,
            r.Segment, r.SizeCategory,
            r.City, r.State,
            r.ReferralCode, r.AssignedPartnerId, r.DistributionMode,
            r.ReviewNotes, r.ReviewedBy, r.ReviewedAt,
            r.ExternalCompanyId, r.ExternalUserId,
            r.IpAddress, r.SubmittedAt, r.UpdatedAt))
            .ToList();
    }
}
