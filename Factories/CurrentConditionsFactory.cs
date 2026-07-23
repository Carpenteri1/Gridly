using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories;

public class CurrentConditionsFactory
{
    public static CurrentConditionsModel Create(CurrentConditionsDtoModel dto)
        => new()
        {
            Conditions = dto.Conditions,
            Temp = dto.Temp,
            FeelsLike = dto.FeelsLike,
            Humidity = dto.Humidity,
            WindSpeed = dto.WindSpeed,
            WindDir = dto.WindDir,
        };
}
