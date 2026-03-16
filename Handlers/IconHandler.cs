using Gridly.Command;
using Gridly.Constants;
using Gridly.Dtos;
using Gridly.Services;
using MediatR;
using System.Text.Json;

public class ComponentHandler(
    IIconRepository iconRepository, 
    IHttpClientServices httpClientServices) : 
        IRequestHandler<GetIconCommand, IResult>,
        IRequestHandler<SearchIconsCommand, IResult>
{   
    public async Task<IResult> Handle(GetIconCommand command, CancellationToken cancellationToken)
    {

        //TODO Thirdparty endpoint here
        //Some limiter here
        //Search for stored icons here
        return Results.Ok();
    }

    public async Task<IResult> Handle(SearchIconsCommand command, CancellationToken cancellationToken)
    {
        //Some limiter here
        //cashing maybe??
        var result = await httpClientServices.Get(string.Format(EndpointStrings.SearchIconifyDesignEndPoint, command.Value));
        var searchResult = JsonSerializer.Deserialize<SearchIconsResultDto>(result.Item2, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return Results.Ok(searchResult);
    }
}