namespace Gridly.Entities;

public class IconEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Base64Data { get; set; }
    public string? MaterialIcon { get; set; }

    public ICollection<IconsConnectedEntity> IconsConnected { get; set; } = [];
}
