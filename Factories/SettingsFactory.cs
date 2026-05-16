using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories
{
    public static class SettingsFactory
    {
        public static SettingsModel Create(SettingsModel? settings)
       => new SettingsModel
       {
            Width = settings?.Width ?? 250,
            Height = settings?.Height ?? 250,
            TitleHidden = settings?.TitleHidden ?? false,
            ImageHidden = settings?.ImageHidden ?? false
       };
    }
}
