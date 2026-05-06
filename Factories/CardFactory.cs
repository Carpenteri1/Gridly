using Gridly.Dtos;
using Gridly.Models;

namespace Gridly.Factories;

public static class CardFactory
{
    public static CardModel Create(CardDtoModel dto) 
        => new()
        {
            Id = dto.CardId,
            IndexPosition = dto. IndexPosition,
            Name = dto.CardName,
            Url = dto.Url,
            IconUrl = dto.IconUrl,
            IconData = new IconModel
            {
                Id = dto.IconId,
                Name = dto.IconName,
                Type = dto.Type,
                Base64Data = dto.Base64Data,
                MaterialIcon = dto.MaterialIcon
            },
            Settings = new SettingsModel
            {
                Id = dto.SettingsId,
                CardId = dto.CardId,
                Width = dto.Width,
                Height = dto.Height,
                TitleHidden = dto.TitleHidden,
                ImageHidden = dto.ImageHidden
            }
        };

    public static IEnumerable<CardModel> CreateMany(IEnumerable<CardDtoModel> dtos) 
        => dtos.Select(Create);
}
