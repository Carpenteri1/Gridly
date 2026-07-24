using System.Text.Json.Serialization;

namespace Gridly.Models;

public class WeatherModel 
{ 
    public string Location { get; set; } 
    public string Address { get; set; }
    public string Timezone { get; set; }
    public string Description { get; set; }
    public CurrentConditionsModel CurrentConditions { get; set; }
}