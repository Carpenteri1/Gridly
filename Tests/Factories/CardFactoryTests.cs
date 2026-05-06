using Gridly.Dtos;
using Gridly.Factories;

namespace Gridly.Tests.Factories;

public class CardFactoryTests
{
    [Fact]
    public void Create_MapsDtoFieldsToCardModel()
    {
        var dto = new CardDtoModel
        {
            CardId = 12,
            IndexPosition = 3,
            CardName = "Docs",
            Url = "https://example.test",
            IconUrl = "/icons/docs.svg"
        };

        var result = CardFactory.Create(dto);

        Assert.Equal(dto.CardId, result.Id);
        Assert.Equal(dto.IndexPosition, result.IndexPosition);
        Assert.Equal(dto.CardName, result.Name);
        Assert.Equal(dto.Url, result.Url);
        Assert.Equal(dto.IconUrl, result.IconUrl);
        Assert.NotNull(result.IconData);
        Assert.NotNull(result.Settings);
    }

    [Fact]
    public void CreateMany_PreservesOrderAndCount()
    {
        var dtos = new[]
        {
            new CardDtoModel { CardId = 1, CardName = "First" },
            new CardDtoModel { CardId = 2, CardName = "Second" }
        };

        var result = CardFactory.CreateMany(dtos).ToArray();

        Assert.Equal(2, result.Length);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("First", result[0].Name);
        Assert.Equal(2, result[1].Id);
        Assert.Equal("Second", result[1].Name);
    }

    [Fact]
    public void Create_MapsIconAndSettings()
    {
        var dto = new CardDtoModel
        {
            CardId = 8,
            IconId = 21,
            IconName = "grid",
            Type = "svg",
            Base64Data = "Zm9v",
            MaterialIcon = "dashboard",
            SettingsId = 13,
            Width = 500,
            Height = 300,
            TitleHidden = true,
            ImageHidden = false
        };

        var result = CardFactory.Create(dto);

        Assert.NotNull(result.IconData);
        Assert.Equal(21, result.IconData.Id);
        Assert.Equal("grid", result.IconData.Name);
        Assert.Equal("svg", result.IconData.Type);
        Assert.Equal("Zm9v", result.IconData.Base64Data);
        Assert.Equal("dashboard", result.IconData.MaterialIcon);

        Assert.NotNull(result.Settings);
        Assert.Equal(13, result.Settings.Id);
        Assert.Equal(8, result.Settings.CardId);
        Assert.Equal(500, result.Settings.Width);
        Assert.Equal(300, result.Settings.Height);
        Assert.True(result.Settings.TitleHidden);
        Assert.False(result.Settings.ImageHidden);
    }
}
