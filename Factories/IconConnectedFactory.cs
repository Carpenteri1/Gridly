using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories
{
    public class IconConnectedFactory
    {
        public static IconConnectedDtoModel Create(int componentId, int iconId)
        => new IconConnectedDtoModel
        {
            IconId = iconId,
            ComponentId = componentId,
        };
    }
}
