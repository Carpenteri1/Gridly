using MediatR;

namespace Gridly.Querys;

public class GetLocalProviderKeyStatusQuery : IRequest<IResult>
{
    public required string Provider { get; set; }
}
