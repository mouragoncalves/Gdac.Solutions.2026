using Gdac.Content.Domain.Entities;
using Gdac.Content.Domain.Exceptions;
using Gdac.Content.Domain.Interfaces.Repositories;
using MediatR;

namespace Gdac.Content.Application.Features.Services.Queries.GetContentService;

public class GetContentServiceHandler(IContentServiceRepository repo)
    : IRequestHandler<GetContentServiceQuery, ContentServiceResult>
{
    public async Task<ContentServiceResult> Handle(GetContentServiceQuery request, CancellationToken ct)
    {
        var s = await repo.GetByIdWithDetailsAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(ContentService), request.Id);

        return MapToResult(s);
    }

    internal static ContentServiceResult MapToResult(ContentService s) => new(
        s.Id, s.Name, s.Category, s.Description,
        s.IsActive, s.IsFeatured, s.DisplayOrder,
        s.PrecoRevenda, s.PrecoSugeridoFinal,
        s.DescontoSugeridoSemestral, s.DescontoSugeridoAnual,
        s.CreatedAt, s.UpdatedAt,
        s.Media.OrderBy(m => m.DisplayOrder)
            .Select(m => new ServiceMediaResult(m.Id, m.Url, m.Type, m.DisplayOrder))
            .ToList());
}
