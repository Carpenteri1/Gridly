using Gridly.Dtos;
using Gridly.Entities;

namespace Gridly.Factories
{
    public class WeatherDataConnectionFactory
    {
        public static WeatherDataConnectionEntity Create(int cardId, int weatherId)
            => new()
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
