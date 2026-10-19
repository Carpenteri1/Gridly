using Gridly.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Extension;

public static class RowColumnQueryExtensions
{
    public static IQueryable<ColumnRowEntity> GetRowsAndConnectedCards(
        this IQueryable<ColumnRowEntity> entities)
    {
        return entities
            .Include(row => row.Cards!.OrderBy(card => card.IndexPosition))
            .ThenInclude(card => card.Settings)
            .Include(row => row.Cards!.OrderBy(card => card.IndexPosition))
            .ThenInclude(card => card.IconsConnected)
            .ThenInclude(iconsConnected => iconsConnected!.Icon);
    }
}