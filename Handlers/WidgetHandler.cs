using Gridly.Data;
using Gridly.Factories;
using Gridly.Querys;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Handlers;

public class WidgetHandler(GridlyDbContext dbContext) : IRequestHandler<GetWidgetQuery, IResult>
{
    public async Task<IResult> Handle(GetWidgetQuery query, CancellationToken cancellationToken)
    {
        var widgetsEntities = dbContext.Widgets.AsNoTracking();
        var widgets = WidgetFactory.CreateMany(widgetsEntities).ToList();

        return widgets.Any() ? Results.Ok(widgets.ToList()) : Results.NoContent();
    }
}