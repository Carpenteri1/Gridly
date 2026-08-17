using System.Data;
using Dapper;
using Gridly.Data;
using Gridly.Models;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;

namespace Gridly.Tests.Repositories;

public sealed class SettingsRepositoryTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-settings-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;

    public SettingsRepositoryTests()
    {
        _connection = new SqliteConnection($"Data Source={_dbPath}");
    }

    private async Task<int> SeedCardAsync()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var rowColumnId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO RowColumn (RowPosition, RowWidth) VALUES (1,1); SELECT last_insert_rowid();");
        var cardId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO Card (IndexPosition, RowColumnId, Name, Url, Type, IconUrl) VALUES (1, @row, 'A', '', '', ''); SELECT last_insert_rowid();",
            new { row = rowColumnId });
        return (int)cardId;
    }

    [Fact]
    public async Task Insert_CreatesSettingsAndReturnsGeneratedId()
    {
        var cardId = await SeedCardAsync();
        var repository = new SettingsRepository(_connection);

        var result = await repository.Insert(new SettingsModel { CardId = cardId, Width = 250, Height = 250 });

        Assert.True(result.Id > 0);
        Assert.Equal(cardId, result.CardId);
        Assert.Equal(250, result.Width);
    }

    [Fact]
    public async Task Edit_UpdatesStoredWidthAndHeight()
    {
        var cardId = await SeedCardAsync();
        var repository = new SettingsRepository(_connection);
        var inserted = await repository.Insert(new SettingsModel { CardId = cardId, Width = 250, Height = 250 });

        await repository.Edit(new SettingsModel { Id = inserted.Id, CardId = cardId, Width = 500, Height = 400, TitleHidden = true });

        var stored = await _connection.QuerySingleAsync<SettingsModel>(
            "SELECT Id, CardId, Width, Height, TitleHidden, ImageHidden FROM Settings WHERE CardId = @CardId;",
            new { CardId = cardId });
        Assert.Equal(500, stored.Width);
        Assert.Equal(400, stored.Height);
        Assert.True(stored.TitleHidden);
    }

    [Fact]
    public async Task Delete_RemovesSettingsForCard()
    {
        var cardId = await SeedCardAsync();
        var repository = new SettingsRepository(_connection);
        await repository.Insert(new SettingsModel { CardId = cardId, Width = 250, Height = 250 });

        var result = await repository.Delete(cardId);

        Assert.True(result);
        var remaining = await _connection.QuerySingleAsync<int>(
            "SELECT COUNT(*) FROM Settings WHERE CardId = @CardId;", new { CardId = cardId });
        Assert.Equal(0, remaining);
    }

    public void Dispose()
    {
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        if (File.Exists(_dbPath + ".bak")) File.Delete(_dbPath + ".bak");
    }
}
