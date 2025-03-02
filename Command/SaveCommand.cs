using MediatR;

namespace Gridly.Models;

public class SaveCommand : ComponentModel, IRequest<ComponentModel[]>
{
}