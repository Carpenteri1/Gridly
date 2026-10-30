using Gridly.Querys;
using Gridly.Constants;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Handlers;

public class IconHandler(
    IHttpClientServices httpClientServices,
    IMemoryCashingService memoryCache,
    GridlyDbContext dbContext) : 
        IRequestHandler<GetIconQuery, IResult>,
        IRequestHandler<SearchIconsQuery, IResult>
{
    public async Task<IResult> Handle(GetIconQuery query, CancellationToken cancellationToken)
    {
        var iconEntities = await dbContext.Icons
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return iconEntities.Any() ? Results.Ok(iconEntities) : Results.NoContent();
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