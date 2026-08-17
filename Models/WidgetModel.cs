namespace Gridly.Models;

public class WidgetModel
{
    public int Id {get; set;}
    public required string WidgetType {get; set;}
    public required string Label {get; set;}
    public required string Description {get; set;}
    public required string Icon {get; set;}
}