using MediatR;

namespace Gridly.Models;

public class GetAllCommand : IRequest<ComponentModel[]>
{
}