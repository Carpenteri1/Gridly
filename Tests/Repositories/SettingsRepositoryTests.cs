using Gridly.Data;
using Gridly.Entities;
using Gridly.Models;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Tests.Repositories;

public sealed class SettingsRepositoryInsertTests
{
    [Fact]
    public async Task Insert_WhenSettingsIsValid_PersistsRowAndReturnsSettingsWithGeneratedId()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connection).Options);
        await dbContext.Database.EnsureCreatedAsync();
        var repository = new SettingsRepository(dbContext);

        var card = new CardEntity
        {
            IndexPosition = 1, RowColumnId = 1, Name = "Docs", Url = "", IconUrl = "", Type = "",
        };
        dbContext.Cards.Add(card);
        await dbContext.SaveChangesAsync();

        var settings = new SettingsModel
        {
            CardId = card.Id, Width = 400, Height = 300, TitleHidden = true, ImageHidden = false,
        };

        var result = await repository.Insert(settings);

        Assert.NotNull(result.Id);
        Assert.Equal(card.Id, result.CardId);
        Assert.Equal(400, result.Width);
        Assert.Equal(300, result.Height);
        Assert.True(result.TitleHidden);
        Assert.False(result.ImageHidden);

        var persisted = await dbContext.Settings.SingleAsync(s => s.Id == result.Id);
        Assert.Equal(card.Id, persisted.CardId);
        Assert.Equal(400, persisted.Width);
        Assert.Equal(300, persisted.Height);
    }
}

public sealed class SettingsRepositoryEditTests
{
    [Fact]
    public async Task Edit_WhenSettingsExists_UpdatesAndReturnsUpdatedSettings()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connection).Options);
        await dbContext.Database.EnsureCreatedAsync();
        var repository = new SettingsRepository(dbContext);

        var card = new CardEntity
        {
            IndexPosition = 1, RowColumnId = 1, Name = "Docs", Url = "", IconUrl = "", Type = "",
        };
        dbContext.Cards.Add(card);
        await dbContext.SaveChangesAsync();
        var existing = new SettingsEntity
        {
            CardId = card.Id, Width = 250, Height = 250, TitleHidden = false, ImageHidden = false,
        };
        dbContext.Settings.Add(existing);
        await dbContext.SaveChangesAsync();

        var result = await repository.Edit(new SettingsModel
        {
            CardId = card.Id, Width = 500, Height = 450, TitleHidden = true, ImageHidden = true,
        });

        Assert.Equal(existing.Id, result.Id);
        Assert.Equal(card.Id, result.CardId);
        Assert.Equal(500, result.Width);
        Assert.Equal(450, result.Height);
        Assert.True(result.TitleHidden);
        Assert.True(result.ImageHidden);

        var reloaded = await dbContext.Settings.AsNoTracking().SingleAsync(s => s.Id == existing.Id);
        Assert.Equal(500, reloaded.Width);
        Assert.Equal(450, reloaded.Height);
        Assert.Equal(card.Id, reloaded.CardId);
    }

    [Fact]
    public async Task Edit_WhenSettingsDoesNotExist_ReturnsInputUnchanged()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connection).Options);
        await dbContext.Database.EnsureCreatedAsync();
        var repository = new SettingsRepository(dbContext);

        var settings = new SettingsModel
        {
            CardId = 999, Width = 500, Height = 450, TitleHidden = true, ImageHidden = true,
        };

        var result = await repository.Edit(settings);

        Assert.Same(settings, result);
        Assert.Empty(await dbContext.Settings.AsNoTracking().ToListAsync());
    }
}

public sealed class SettingsRepositoryDeleteTests
{
    [Fact]
    public async Task Delete_WhenSettingsExists_RemovesRowAndReturnsTrue()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connection).Options);
        await dbContext.Database.EnsureCreatedAsync();
        var repository = new SettingsRepository(dbContext);

        var card = new CardEntity
        {
            IndexPosition = 1, RowColumnId = 1, Name = "Docs", Url = "", IconUrl = "", Type = "",
        };
        dbContext.Cards.Add(card);
        await dbContext.SaveChangesAsync();
        dbContext.Settings.Add(new SettingsEntity { CardId = card.Id, Width = 250, Height = 250 });
        await dbContext.SaveChangesAsync();

        var result = await repository.Delete(card.Id);

        Assert.True(result);
        Assert.False(await dbContext.Settings.AnyAsync(s => s.CardId == card.Id));
    }

    [Fact]
    public async Task Delete_WhenSettingsDoesNotExist_ReturnsFalse()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connection).Options);
        await dbContext.Database.EnsureCreatedAsync();
        var repository = new SettingsRepository(dbContext);

        var result = await repository.Delete(999);

        Assert.False(result);
    }
}
