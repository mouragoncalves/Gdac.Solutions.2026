using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IRequest<GetUserByIdResult>;

public record GetUserByIdResult(Guid Id, string Email, bool IsActive, bool MustChangePassword,
    int FailedLoginAttempts, DateTime? LockoutUntil, DateTime CreatedAt, DateTime UpdatedAt);
