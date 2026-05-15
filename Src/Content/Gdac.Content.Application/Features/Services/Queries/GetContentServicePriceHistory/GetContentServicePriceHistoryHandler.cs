using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Services.Queries.GetContentServicePriceHistory;

public class GetContentServicePriceHistoryHandler(IContentServiceRepository repo)
    : IRequestHandler<GetContentServicePriceHistoryQuery, IReadOnlyList<ContentServicePriceHistoryResult>>
{
    public async Task<IReadOnlyList<ContentServicePriceHistoryResult>> Handle(
        GetContentServicePriceHistoryQuery request, CancellationToken ct)
    {
        var s = await repo.GetByIdWithDetailsAsync(request.ServiceId, ct)
            ?? throw new NotFoundException(nameof(ContentService), request.ServiceId);

        return s.PriceHistory
            .OrderByDescending(h => h.ChangedAt)
            .Select(h => new ContentServicePriceHistoryResult(
                h.Id, h.PrecoRevenda, h.PrecoSugeridoFinal,
                h.DescontoSugeridoSemestral, h.DescontoSugeridoAnual,
                h.ChangedBy, h.ChangedAt, h.Notes))
            .ToList();
    }
}
