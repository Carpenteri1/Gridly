namespace Gridly.Dtos;

public class ClockDataDtoModel
{
    public int Id { get; set; }
    public string Location { get; set; }
    public string JsonPayload { get; set; }
    public DateTime FetchedAt { get; set; }
}
