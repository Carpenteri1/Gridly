namespace Gridly.Dtos;

public class CardDtoModel
{
    public int CardId { get; set; }
    public int IndexPosition { get; set; }
    public string CardName { get; set; }
    public string Url { get; set; }
    public string IconUrl { get; set; }
    public int IconId { get; set; }
    public string IconName { get; set; }
    public string Type { get; set; }
    public string Base64Data { get; set; }
    public string MaterialIcon { get; set; }
    public int SettingsId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool TitleHidden { get; set; }
    public bool ImageHidden { get; set; }
}