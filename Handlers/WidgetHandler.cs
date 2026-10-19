using Gridly.Data;
using Gridly.Querys;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Handlers;

public class WidgetHandler(GridlyDbContext dbContext) : IRequestHandler<GetWidgetQuery, IResult>
{
    public async Task<IResult> Handle(GetWidgetQuery query, CancellationToken cancellationToken)
    {
        var widgets = await dbContext.Widgets
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return widgets.Any() ? Results.Ok(widgets.ToList()) : Results.NoContent();
    }
}