using System.Text.Json.Serialization;

namespace Gridly.Models;

public class EditComponentModel  
{
    [JsonPropertyName("editComponent")] public ComponentModel? EditComponent { get; set; }
    [JsonPropertyName("selectedDropDownIconValue")] public int? SelectedDropDownIconValue { get; set; }
}