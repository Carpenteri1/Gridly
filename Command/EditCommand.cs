using MediatR;

namespace Gridly.Models;

public class EditCommand : EditComponentForm, IRequest<IResult>
{
    
}