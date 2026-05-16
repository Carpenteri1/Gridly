using Gridly.Command;
using Gridly.Constants;
using Gridly.Dtos;
using Gridly.Services;
using MediatR;

public class CardHandler(
    IHttpClientServices httpClientServices,
    IMemoryCashingService memoryCache) : 
        IRequestHandler<GetIconCommand, IResult>,
        IRequestHandler<SearchIconsCommand, IResult>
{
    public async Task<IResult> Handle(GetIconCommand command, CancellationToken cancellationToken)
    {
        return Results.Ok();
    }

    public async Task<IResult> Handle(SearchIconsCommand command, CancellationToken cancellationToken)
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
            .Where(icon => icon.Contains(command.Value, StringComparison.OrdinalIgnoreCase))
            .Take(50)
            .ToArray();

        return Results.Ok(new SearchIconsResultDto { Icons = matches });
    }
}