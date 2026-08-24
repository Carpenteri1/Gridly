using Gridly.Entities;

namespace Gridly.Extension;

public static class IconConnectQueryExtensions
{
    public static IQueryable<IconsConnectedEntity> WhereId(
        this IQueryable<IconsConnectedEntity> connection, int? cardId, int? iconId)
    {
        if(cardId != null)
            connection = connection.Where(c => c.CardId == cardId);
        if(iconId != null) 
            connection = connection.Where(c => c.IconId == iconId);
        return connection;
    }
}