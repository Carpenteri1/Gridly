using MediatR;

namespace Gridly.Querys;

public class GetRemoteProviderKeyStatusQuery : IRequest<IResult>
{
    public required string Provider { get; set; }
}
