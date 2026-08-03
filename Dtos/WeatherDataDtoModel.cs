namespace Gridly.Dtos;

public class WeatherDataDtoModel
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public string Location { get; set; }
    public string Address { get; set; }
    public string Timezone { get; set; }
    public string Description { get; set; }
    public string Conditions { get; set; }
    public double Temp { get; set; }
    public double FeelsLike { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public double WindDir { get; set; }
    public DateTime FetchedAt { get; set; }
}
