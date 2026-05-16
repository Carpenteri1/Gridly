using Gridly.Models;
using MediatR;

namespace Gridly.Command;

public class GetCardByIdCommands : IRequest<CardModel>
{
    public int Id { get; set; }
}