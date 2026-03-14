using Gridly.Command;
using Gridly.Services;
using MediatR;

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

        //https://api.iconify.design/
        return Results.Ok();
    }

    public async Task<IResult> Handle(SearchIconsCommand command, CancellationToken cancellationToken)
    {
        //Some limiter here
        //cashing maybe??
        var endpoint = $"https://api.iconify.design/search?query={command.Value}&limit=20";
        var result = await httpClientServices.Get(endpoint);
        return Results.Ok(result);
    }
}