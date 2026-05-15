using Gdac.Onboarding.Domain.Enums;
using MediatR;

namespace Gdac.Onboarding.Application.Features.Registrations.Commands.SubmitClientRegistration;

public record SubmitClientRegistrationCommand(
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
    string? ReferralCode,
    string? IpAddress
) : IRequest<Guid>;
