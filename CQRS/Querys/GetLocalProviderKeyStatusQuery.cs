using MediatR;

namespace Gridly.Querys;

public class GetLocalProviderKeyStatusQuery : IRequest<IResult>
{
    public string Provider { get; set; }
}
