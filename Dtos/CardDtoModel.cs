namespace Gridly.Dtos;

public class CardDtoModel
{
    public int CardId { get; set; }
    public int IndexPosition { get; set; }
    public int RowColumnId { get; set; }
    public required string CardName { get; set; }
    public required string CardType { get; set; }
    public required string Url { get; set; }
    public required string IconUrl { get; set; }
    public int IconId { get; set; }
    public required string IconName { get; set; }
    public required string Type { get; set; }
    public required string Base64Data { get; set; }
    public required string MaterialIcon { get; set; }
    public int SettingsId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool TitleHidden { get; set; }
    public bool ImageHidden { get; set; }
}