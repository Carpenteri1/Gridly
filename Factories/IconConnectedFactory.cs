using Gridly.Dtos;
using Gridly.Entities;

namespace Gridly.Factories
{
    public class IconConnectedFactory
    {
        public static IconConnectedDtoModel Create(int cardId, int iconId)
        => new IconConnectedDtoModel
        {
            IconId = iconId,
            CardId = cardId,
        };

        public static IconConnectedDtoModel Create(IconsConnectedEntity entity)
        => new IconConnectedDtoModel
        {
            Id = entity.Id,
            IconId = entity.IconId,
            CardId = entity.CardId,
        };
    }
}
