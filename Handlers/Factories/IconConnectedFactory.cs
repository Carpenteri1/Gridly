using Gridly.Dtos;

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
    }
}
