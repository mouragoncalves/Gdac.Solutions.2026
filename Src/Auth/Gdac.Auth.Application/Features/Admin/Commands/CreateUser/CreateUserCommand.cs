using MediatR;

namespace Gdac.Auth.Application.Features.Admin.Commands.CreateUser;

public record CreateUserCommand(string Email, bool MustChangePassword) : IRequest<CreateUserResult>;
public record CreateUserResult(Guid Id, string Email, string TemporaryPassword);
