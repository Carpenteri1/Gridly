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
            IconUrl = dto.IconUrl,
            IconData = new IconModel
            {
                Id = dto.IconId,
                Name = dto.IconName,
                Type = dto.Type,
                Base64Data = dto.Base64Data,
                MaterialIcon = dto.MaterialIcon
            },
            ComponentSettings = new ComponentSettingsModel
            {
                Id = dto.ComponentSettingsId,
                ComponentId = dto.ComponentId,
                Width = dto.Width,
                Height = dto.Height,
                TitleHidden = dto.TitleHidden,
                ImageHidden = dto.ImageHidden
            }
        };

    public static IEnumerable<ComponentModel> CreateMany(IEnumerable<ComponentDtoModel> dtos) 
        => dtos.Select(Create);
}
