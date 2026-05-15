using MediatR;

namespace Gdac.Core.Application.Features.BlockList.Queries.CheckCnpjBlocked;

public record CheckCnpjBlockedQuery(string CnpjBase) : IRequest<bool>;
