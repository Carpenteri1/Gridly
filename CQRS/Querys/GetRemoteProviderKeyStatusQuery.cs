using MediatR;

namespace Gridly.Querys;

public class GetRemoteProviderKeyStatusQuery : IRequest<IResult>
{
    public string Provider { get; set; }
}
