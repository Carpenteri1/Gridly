using Gridly.Entities;
using Gridly.Models;

namespace Gridly.Factories;

public class WidgetFactory
{
    
    public static WidgetModel Create(WidgetEntity entity) 
        => new()
        {
            Id = entity.Id,
            WidgetType = entity.WidgetType,
            Label = entity.Label,
            Description = entity.Description,
            Icon = entity.Icon,
        };

    public static IEnumerable<WidgetModel> CreateMany(IEnumerable<WidgetEntity> dtos) 
        => dtos.Select(Create);
}