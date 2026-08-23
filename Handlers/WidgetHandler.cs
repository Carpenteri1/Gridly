using Gridly.Querys;
using Gridly.Repositories;
using Gridly.Repositories.Interfaces;
using MediatR;

namespace Gridly.Handlers;

public class WidgetHandler(IWidgetRepository widgetRepository) : IRequestHandler<GetWidgetQuery, IResult>
{
    public async Task<IResult> Handle(GetWidgetQuery query, CancellationToken cancellationToken)
    {
        var widgets =  await widgetRepository.Get();
        return widgets.Any() ? Results.Ok(widgets.ToList()) : Results.NoContent();
    }
}