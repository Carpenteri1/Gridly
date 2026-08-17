namespace Gridly.Entities;

public class SettingsEntity
{
    public int Id { get; set; }
    public int CardId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool TitleHidden { get; set; }
    public bool ImageHidden { get; set; }
}
