namespace Gridly.Dtos;

public class WidgetDtoModel
{
    public int Id {get; set;}
    public int WidgetType {get; set;}
    public required string Label {get; set;}
    public required string Description {get; set;}
    public required string Icon {get; set;}
}