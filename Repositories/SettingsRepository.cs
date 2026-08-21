using Gridly.Data;
using Gridly.Entities;
using Gridly.Factories;
using Gridly.Models;
using Gridly.Services;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Repositories;

public class SettingsRepository(GridlyDbContext dbContext) : ISettingsRepository
{
    public async Task<SettingsModel> Insert(SettingsModel settings)
    {
        var entity = new SettingsEntity
        {
            CardId = settings.CardId!.Value,
            Width = settings.Width,
            Height = settings.Height,
            TitleHidden = settings.TitleHidden,
            ImageHidden = settings.ImageHidden,
        };
        dbContext.Settings.Add(entity);
        await dbContext.SaveChangesAsync();
        return SettingsFactory.Create(entity);
    }

    public async Task<SettingsModel> Edit(SettingsModel settings)
    {
        var entity = await dbContext.Settings.FirstOrDefaultAsync(s => s.CardId == settings.CardId);
        if (entity is null)
            return settings;

        entity.Width = settings.Width;
        entity.Height = settings.Height;
        entity.TitleHidden = settings.TitleHidden;
        entity.ImageHidden = settings.ImageHidden;

        await dbContext.SaveChangesAsync();
        return SettingsFactory.Create(entity);
    }

    public async Task<bool> Delete(int Id)
    {
        var result = await dbContext.Settings.Where(s => s.CardId == Id).ExecuteDeleteAsync();
        return result > 0;
    }
}
