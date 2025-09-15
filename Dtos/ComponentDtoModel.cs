namespace Gridly.Dtos;

public class ComponentDtoModel
{
    public int ComponentId { get; set; }
    public string ComponentName { get; set; }
    public string Url { get; set; }
    public string IconUrl { get; set; }
    public bool TitleHidden { get; set; }
    public bool ImageHidden { get; set; }
    public int IconId { get; set; }
    public string IconName { get; set; }
    public string Type { get; set; }
    public string Base64Data { get; set; }
    public int ComponentSettingsId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}