import {WeatherModel} from "../models/weather.Model";
import {WeatherDataDto} from "../dtos/weatherDataDto";

export class WeatherDataDtoFactory {
  public static createDto(weather: WeatherModel): WeatherDataDto {
    const dto = new WeatherDataDto();

    dto.cardId = weather.cardId;
    dto.location = weather.location;
    dto.address = weather.address;
    dto.timezone = weather.timezone;
    dto.description = weather.description;
    dto.conditions = weather.currentConditions.conditions;
    dto.temp = weather.currentConditions.temp;
    dto.feelsLike = weather.currentConditions.feelsLik;
    dto.humidity = weather.currentConditions.humidity;
    dto.windSpeed = weather.currentConditions.windspeed;
    dto.windDir = weather.currentConditions.windDir;

    return dto;
  }
}
