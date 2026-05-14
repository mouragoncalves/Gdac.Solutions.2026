using MediatR;

namespace Gdac.Core.Application.Features.UserProfiles.Commands.CreateUserProfile;

public record CreateUserProfileCommand(Guid UserId, string FullName, string Email) : IRequest<Guid>;
