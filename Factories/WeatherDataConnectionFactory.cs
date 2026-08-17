using Gridly.Dtos;

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
    }
}
