namespace Gridly.Entities;

public class CardEntity
{
    public int Id { get; set; }
    public int IndexPosition { get; set; }
    public int RowColumnId { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public string? Type { get; set; }
    public string? IconUrl { get; set; }

    public ICollection<SettingsEntity> Settings { get; set; } = [];
    public ICollection<IconsConnectedEntity> IconsConnected { get; set; } = [];
}
