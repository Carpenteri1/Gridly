using Gridly.Dtos;
using Gridly.Entities;
using Gridly.Models;

namespace Gridly.Factories;

public static class CardFactory
{
    public static CardModel Create(CardEntity entity)
    {
        var icon = entity.IconsConnected?.Icon;

        return new CardModel
        {
            Id = entity.Id,
            IndexPosition = entity.IndexPosition,
            RowColumnId = entity.RowColumnId,
            Name = entity.Name,
            Url = entity.Url,
            Type = entity.Type,
            IconUrl = entity.IconUrl,
            Settings = entity.Settings is null
                ? null
                : new SettingsModel
                {
                    Id = entity.Settings.Id,
                    CardId = entity.Settings.CardId,
                    Width = entity.Settings.Width,
                    Height = entity.Settings.Height,
                    TitleHidden = entity.Settings.TitleHidden ?? false,
                    ImageHidden = entity.Settings.ImageHidden ?? false,
                },
            IconData = icon is null
                ? null
                : new IconModel
                {
                    Id = icon.Id,
                    Name = icon.Name!,
                    Type = icon.Type!,
                    Base64Data = icon.Base64Data!,
                    MaterialIcon = icon.MaterialIcon!,
                },
        };
    }
    
    public static CardEntity Create(CardModel card)
        => new()
        {
            IndexPosition = card.IndexPosition,
            RowColumnId = card.RowColumnId,
            Name = card.Name,
            Url = card.Url,
            Type = card.Type,
            IconUrl = card.IconUrl,
        };

    public static CardModel Create(CardDtoModel dto)
        => new()
        {
            Id = dto.CardId,
            IndexPosition = dto. IndexPosition,
            RowColumnId = dto.RowColumnId,
            Name = dto.CardName,
            Url = dto.Url,
            Type = dto.CardType,
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
