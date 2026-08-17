using MediatR;

namespace Gridly.Commands;

public class SaveProviderKeyCommand : IRequest<IResult>
{
    public required string Provider { get; set; }
    public required string RawKey { get; set; }
}
