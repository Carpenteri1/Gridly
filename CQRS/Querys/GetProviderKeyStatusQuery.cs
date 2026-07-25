using MediatR;

namespace Gridly.Querys;

public class GetProviderKeyStatusQuery : IRequest<IResult>
{
    public string Provider { get; set; }
}
