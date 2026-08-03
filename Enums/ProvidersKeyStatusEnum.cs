using System.Text.Json.Serialization;

namespace Gridly.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProvidersKeyStatusEnum
{
    Unknown,
    Valid,
    Invalid,
}
