using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories;

public static class ComponentFactory
{
    public static ComponentModel Create(ComponentDtoModel dto) 
        => new()
        {
            Id = dto.ComponentId,
            IndexPosition = dto. IndexPosition,
            Name = dto.ComponentName,
            Url = dto.Url,
            IconUrl = dto.IconUrl
        };

    public static IEnumerable<ComponentModel> CreateMany(IEnumerable<ComponentDtoModel> dtos) 
        => dtos.Select(Create);
}
