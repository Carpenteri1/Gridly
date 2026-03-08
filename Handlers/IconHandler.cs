using Gridly.Command;
using Gridly.Services;
using MediatR;

public class ComponentHandler(
        IIconRepository iconRepository) : 
        IRequestHandler<GetIconsCommand, IResult>
{   
    public async Task<IResult> Handle(GetIconsCommand command, CancellationToken cancellationToken)
    {
        //TODO Thirdparty endpoint here
        //Some limiter here
        //Search for stored icons here
        return Results.Ok();
    }
}