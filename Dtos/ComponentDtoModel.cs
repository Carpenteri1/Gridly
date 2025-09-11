using Gridly.Models;

namespace Gridly.Dtos;

public class ComponentDtoModel : ComponentModel
{
    public string IconName { get; set; }
    public string Type { get; set; }
    public string Base64Data { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}