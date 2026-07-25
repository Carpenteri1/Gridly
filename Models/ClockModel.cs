namespace Gridly.Models;

public class ClockModel
{
    public string TimeZone { get; set; }
    public int UtcOffsetSeconds { get; set; }
    public bool DstActive { get; set; }
}
