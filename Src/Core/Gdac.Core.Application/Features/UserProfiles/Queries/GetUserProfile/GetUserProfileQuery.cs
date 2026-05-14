using MediatR;

namespace Gdac.Core.Application.Features.UserProfiles.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileResult>;

public record UserProfileResult(
    Guid Id, string FullName, string Email,
    string? Phone, string? AvatarUrl, string? Cpf,
    DateOnly? BirthDate, bool IsActive,
    DateTime CreatedAt, DateTime UpdatedAt);
