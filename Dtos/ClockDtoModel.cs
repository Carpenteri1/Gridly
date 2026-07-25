using System.Text.Json.Serialization;

namespace Gridly.Dtos;

public class ClockDtoModel
{
    [JsonPropertyName("timezone")] public string TimeZone { get; set; }
    [JsonPropertyName("utc_offset_seconds")] public int UtcOffsetSeconds { get; set; }
    [JsonPropertyName("dst_active")] public bool DstActive { get; set; }
}
