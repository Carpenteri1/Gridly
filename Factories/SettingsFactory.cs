using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories
{
    public static class SettingsFactory
    {
        public static ComponentSettingsModel Create(ComponentSettingsModel? settings)
       => new ComponentSettingsModel
       {
            Width = settings?.Width ?? 250,
            Height = settings?.Height ?? 250,
            TitleHidden = settings?.TitleHidden ?? false,
            ImageHidden = settings?.ImageHidden ?? false
       };
    }
}
