using MediatR;

namespace Gridly.Commands;

public class SaveApiKeyCommand : IRequest<IResult>
{
    public string Provider { get; set; }
    public string RawKey { get; set; }
}
