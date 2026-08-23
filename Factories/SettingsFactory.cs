using Gridly.Entities;
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

        public static SettingsModel Create(SettingsEntity entity)
       => new SettingsModel
       {
            Id = entity.Id,
            CardId = entity.CardId,
            Width = entity.Width,
            Height = entity.Height,
            TitleHidden = entity.TitleHidden ?? false,
            ImageHidden = entity.ImageHidden ?? false
       };
    }
}
