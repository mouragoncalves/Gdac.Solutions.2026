using MediatR;

namespace Gdac.Core.Application.Features.BlockList.Commands.BlockCnpj;

public record BlockCnpjCommand(string CnpjBase, Guid BlockedBy, string? Reason) : IRequest;
