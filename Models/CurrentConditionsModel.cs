using System.Text.Json.Serialization;

namespace Gridly.Models;
public class CurrentConditionsModel
{
    public string Conditions { get; set; }
    public double Temp { get; set; }
    public double FeelsLike { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public double WindDir { get; set; }
};