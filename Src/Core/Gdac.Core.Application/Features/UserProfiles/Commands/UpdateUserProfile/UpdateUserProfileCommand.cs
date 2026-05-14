using MediatR;

namespace Gdac.Core.Application.Features.UserProfiles.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(
    Guid UserId, string FullName,
    string? Phone, string? AvatarUrl,
    string? Cpf, DateOnly? BirthDate) : IRequest;
