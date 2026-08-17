using Gridly.Entities;

namespace Gridly.Data;

public interface IGridlyDbContext
{
    IQueryable<CardEntity> Cards { get; }
    IQueryable<SettingsEntity> Settings { get; }
    IQueryable<IconsConnectedEntity> IconsConnected { get; }
    IQueryable<IconEntity> Icons { get; }
}
