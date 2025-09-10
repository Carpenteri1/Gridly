using Gridly.Models;

namespace Gridly.Factories;

public static class ComponentFactory
{
    public static ComponentModel Create(ComponentDtoModel dto) 
        => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Url = dto.Url,
            IconUrl = dto.IconUrl,
            TitleHidden = dto.TitleHidden,
            ImageHidden = dto.ImageHidden,
            IconData = new IconModel(dto.Id, dto.IconName, dto.Type, dto.Base64Data),
            ComponentSettings = new ComponentSettingsModel(dto.Id, dto.Width, dto.Height)
        };

    public static IEnumerable<ComponentModel> CreateMany(IEnumerable<ComponentDtoModel> dtos) 
        => dtos.Select(Create);
    
}