using Gdac.Core.Application.Features.UserProfiles.Queries.GetUserProfile;
using MediatR;

namespace Gdac.Core.Application.Features.UserProfiles.Queries.GetUserProfiles;

public record GetUserProfilesQuery : IRequest<IReadOnlyList<UserProfileResult>>;
