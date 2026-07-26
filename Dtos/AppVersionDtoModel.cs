using System.Text.Json.Serialization;

namespace Gridly.Dtos
{
    public class AppVersionDtoModel
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}
