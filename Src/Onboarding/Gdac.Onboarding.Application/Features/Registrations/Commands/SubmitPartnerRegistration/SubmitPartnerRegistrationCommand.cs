using Gdac.Onboarding.Domain.Enums;
using MediatR;

namespace Gdac.Onboarding.Application.Features.Registrations.Commands.SubmitPartnerRegistration;

public record SubmitPartnerRegistrationCommand(
    string ContactName,
    string ContactEmail,
    string? ContactPhone,
    string CompanyName,
    string? TradeName,
    string Cnpj,
    ClientSegment? Segment,
    CompanySize? SizeCategory,
    string? City,
    string? State,
    string? IpAddress
) : IRequest<Guid>;
