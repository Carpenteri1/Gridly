
namespace Gridly.Dtos;
public record WeatherDataDto(string address, string timezone, string description, IEnumerable<DaysDto> days, DateTime fetchedAt);
public record DaysDto(double temp, double feelslike, double humidity, double windspeed, double windDir);