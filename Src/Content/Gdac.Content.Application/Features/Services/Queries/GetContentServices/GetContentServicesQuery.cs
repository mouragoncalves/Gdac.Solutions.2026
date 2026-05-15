using Gdac.Content.Application.Features.Services.Queries.GetContentService;
using MediatR;

namespace Gdac.Content.Application.Features.Services.Queries.GetContentServices;

public record GetContentServicesQuery : IRequest<IReadOnlyList<ContentServiceResult>>;
