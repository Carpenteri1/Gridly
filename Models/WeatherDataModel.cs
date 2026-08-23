
namespace Gridly.Dtos;

public class WeatherDataModel
{
    public int Id { get; set; }
    public required string Address { get; set; }
    public required string Timezone { get; set; }
    public required string Description { get; set; }
    public double Temp { get; set; }
    public double FeelsLike { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public double WindDir { get; set; }
    public DateTime FetchedAt { get; set; }
}
