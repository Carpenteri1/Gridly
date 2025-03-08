using System.Text.Json.Serialization;

namespace Gridly.Models;

public class VersionModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("newRelease")]
    public bool NewRelease { get; set; }
}