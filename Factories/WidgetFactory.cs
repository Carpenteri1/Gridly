using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories;

public class WidgetFactory
{
    public static WidgetModel Create(WidgetDtoModel dto) 
        => new()
        {
            Id = dto.Id,
            WidgetType = dto.WidgetType,
            Label = dto.Label,
            Description = dto.Description,
            Icon = dto.Icon,
        };

    public static IEnumerable<WidgetModel> CreateMany(IEnumerable<WidgetDtoModel> dtos) 
        => dtos.Select(Create);
}