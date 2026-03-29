using Gridly.Dtos;
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
           MaterialIcon = icon?.MaterialIcon ?? "add_box e146"
       };
    }
}
