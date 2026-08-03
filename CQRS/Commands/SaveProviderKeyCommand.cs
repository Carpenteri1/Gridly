using MediatR;

namespace Gridly.Commands;

public class SaveProviderKeyCommand : IRequest<IResult>
{
    public string Provider { get; set; }
    public string RawKey { get; set; }
}
