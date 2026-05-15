using MediatR;

namespace Gdac.Core.Application.Features.BlockList.Commands.UnblockCnpj;

public record UnblockCnpjCommand(string CnpjBase) : IRequest;
