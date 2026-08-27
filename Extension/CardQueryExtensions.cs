using Gridly.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Extension;

public static class CardQueryExtensions
{
    public static IQueryable<CardEntity> WithSettingsAndIcons(this IQueryable<CardEntity> cards) =>
        cards.Include(c => c.Settings)
            .Include(c => c.IconsConnected).ThenInclude(ic => ic!.Icon);

    public static IQueryable<CardEntity> WhereId(this IQueryable<CardEntity> cards, int id) =>
        cards.Where(c => c.Id == id);
}
