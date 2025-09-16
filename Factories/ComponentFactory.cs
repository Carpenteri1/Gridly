using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories;

public static class ComponentFactory
{
    public static ComponentModel Create(ComponentDtoModel dto) 
        => new()
        {
            Id = dto.ComponentId,
            Name = dto.ComponentName,
            Url = dto.Url,
            IconUrl = dto.IconUrl,
            TitleHidden = dto.TitleHidden,
            ImageHidden = dto.ImageHidden,
            IconData = dto.IconName != null ? new IconModel
            {
                Id = dto.IconId != null ? dto.IconId : 0,
                Name = dto.IconName,
                Type = dto.Type,
                Base64Data = dto.Base64Data
            } : null,
            ComponentSettings = new ComponentSettingsModel
            {
                Id = dto.ComponentSettingsId != null ? dto.ComponentSettingsId : 0,
                ComponentId = dto.ComponentId, 
                Width = dto.Width, 
                Height = dto.Height
            }
        };

    public static IEnumerable<ComponentModel> CreateMany(IEnumerable<ComponentDtoModel> dtos) 
        => dtos.Select(Create);
}