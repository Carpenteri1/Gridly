namespace Gridly.Dtos;

public class WeatherDataDtoModel
{
    public int Id { get; set; }
    public string Location { get; set; }
    public string JsonPayload { get; set; }
    public DateTime FetchedAt { get; set; }
}
