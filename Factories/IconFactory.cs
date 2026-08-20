using Gridly.Entities;
using Gridly.Models;

namespace Gridly.Factories
{
    public static class IconFactory
    {
        public static IconModel Create(IconModel? icon)
       => new IconModel
       {
           Name = icon?.Name ?? string.Empty,
           Type = icon?.Type ?? string.Empty,
           Base64Data = icon?.Base64Data ?? string.Empty,
           MaterialIcon = icon?.MaterialIcon ?? "box"
       };

        public static IconModel Create(IconEntity entity)
       => new IconModel
       {
           Id = entity.Id,
           Name = entity.Name ?? string.Empty,
           Type = entity.Type ?? string.Empty,
           Base64Data = entity.Base64Data ?? string.Empty,
           MaterialIcon = entity.MaterialIcon ?? string.Empty
       };
    }
}
