using System.Text.Json.Serialization;

namespace Gridly.Models;

public class EditCardModel  
{
    [JsonPropertyName("editCard")] public CardModel? EditCard { get; set; }
    [JsonPropertyName("selectedDropDownIconValue")] public int? SelectedDropDownIconValue { get; set; }
}