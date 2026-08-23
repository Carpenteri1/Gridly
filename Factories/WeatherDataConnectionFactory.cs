using Gridly.Dtos;
using Gridly.Entities;

namespace Gridly.Factories
{
    public class WeatherDataConnectionFactory
    {
        public static WeatherDataConnectionDtoModel Create(int cardId, int weatherId)
        => new WeatherDataConnectionDtoModel
        {
            CardId = cardId,
            WeatherId = weatherId,
        };

        public static WeatherDataConnectionDtoModel Create(WeatherDataConnectionEntity entity)
        => new WeatherDataConnectionDtoModel
        {
            Id = entity.Id,
            CardId = entity.CardId,
            WeatherId = entity.WeatherId,
        };
    }
}
