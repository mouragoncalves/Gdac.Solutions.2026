using Gdac.Onboarding.Domain.Enums;
using MediatR;

namespace Gdac.Onboarding.Application.Features.Registrations.Queries.GetRegistration;

public record GetRegistrationQuery(Guid Id) : IRequest<RegistrationResult>;

public record RegistrationResult(
    Guid Id,
    RegistrationType Type,
    RegistrationStatus Status,
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
    Guid? AssignedPartnerId,
    LeadDistributionMode? DistributionMode,
    string? ReviewNotes,
    Guid? ReviewedBy,
    DateTime? ReviewedAt,
    Guid? ExternalCompanyId,
    Guid? ExternalUserId,
    string? IpAddress,
    DateTime SubmittedAt,
    DateTime UpdatedAt);
