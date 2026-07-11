using Gridly.Command;
using Gridly.Repositories;
using MediatR;

namespace Gridly.Handlers;

public class WidgetHandler(IWidgetRepository widgetRepository) : IRequestHandler<GetWidgetCommand, IResult>
{
    public async Task<IResult> Handle(GetWidgetCommand command, CancellationToken cancellationToken)
    {
        var widgets =  await widgetRepository.Get();
        return widgets.Any() ? Results.Ok(widgets.ToList()) : Results.NoContent();
    }
}