using Gridly.Data;
using Gridly.Dtos;
using Gridly.Models;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class WidgetRepository(GridlyDbContext dbContext) : IWidgetRepository
{
    public async Task<IEnumerable<WidgetModel>> Get()
    {
        var query =
            from w in dbContext.Widgets.AsNoTracking()
            join wt in dbContext.WidgetTypes.AsNoTracking() on w.WidgetType equals (int?)wt.Id into wtGroup
            from wt in wtGroup.DefaultIfEmpty()
            select new WidgetDtoModel
            {
                Id = w.Id,
                WidgetType = wt != null ? wt.Name! : string.Empty,
                Label = w.Label!,
                Description = w.Description!,
                Icon = w.Icon!,
            };

        var dtos = await query.ToListAsync();
        return Factories.WidgetFactory.CreateMany(dtos);
    }
}
