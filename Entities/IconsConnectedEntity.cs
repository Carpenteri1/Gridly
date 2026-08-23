namespace Gridly.Entities;

public class IconsConnectedEntity
{
    public int Id { get; set; }
    public int? CardId { get; set; }
    public int? IconId { get; set; }

    public CardEntity? Card { get; set; }
    public IconEntity? Icon { get; set; }
}
