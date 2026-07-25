using MediatR;

namespace Gridly.Querys;

public class GetApiKeyStatusQuery : IRequest<IResult>
{
    public string Provider { get; set; }
}
