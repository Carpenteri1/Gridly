using Gridly.Entities;

namespace Gridly.Extension;

public static class IconConnectQueryExtensions
{
    public static IQueryable<IconsConnectedEntity> WhereId(
        this IQueryable<IconsConnectedEntity> iconsConnectedEntities, int? cardId, int? iconId)
    {
        if(cardId != null)
            iconsConnectedEntities = iconsConnectedEntities.Where(c => c.CardId == cardId);
        if(iconId != null) 
            iconsConnectedEntities = iconsConnectedEntities.Where(c => c.IconId == iconId);
        return iconsConnectedEntities;
    }
}   