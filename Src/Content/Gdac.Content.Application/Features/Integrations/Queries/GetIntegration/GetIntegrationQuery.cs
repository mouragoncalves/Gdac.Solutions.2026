using Gdac.Content.Domain.Enums;
using MediatR;

namespace Gdac.Content.Application.Features.Integrations.Queries.GetIntegration;

public record GetIntegrationQuery(Guid Id) : IRequest<IntegrationResult>;

public record IntegrationResult(
    Guid                Id,
    string              Name,
    IntegrationCategory Category,
    string              LogoUrl,
    string              Description,
    string?             ExternalUrl,
    bool                IsActive,
    int                 DisplayOrder,
    DateTime            CreatedAt,
    DateTime            UpdatedAt);
