using Gridly.Querys;
using Gridly.Constants;
using Gridly.Dtos;
using Gridly.Services;
using MediatR;

namespace Gridly.Handlers;

public class IconHandler(
    IHttpClientServices httpClientServices,
    IMemoryCashingService memoryCache) : 
        IRequestHandler<GetIconQuery, IResult>,
        IRequestHandler<SearchIconsQuery, IResult>
{
    public async Task<IResult> Handle(GetIconQuery query, CancellationToken cancellationToken)
    {
        return Results.Ok();
    }

    public async Task<IResult> Handle(SearchIconsQuery query, CancellationToken cancellationToken)
    {
        var data = memoryCache.Get<string[]>("mat-icons");

        if (data == null)
        {
            var (_, result) = await httpClientServices.Get(EndpointStrings.materialIconsEndPoint);
            if (!result.Any()) return Results.NoContent();

            data = result.Split('\n');
            memoryCache.Store("mat-icons", data);
        }

        var matches = data
            .Where(icon => icon.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase))
            .Take(50)
            .ToArray();

        return Results.Ok(new SearchIconsResultDto { Icons = matches });
    }
}