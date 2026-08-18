using System.Data;
using Dapper;
using Gridly.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Tests.Data;

public sealed class GridlyDbContextTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-dbcontext-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;
    private readonly GridlyDbContext _dbContext;

    public GridlyDbContextTests()
    {
        _connection = new SqliteConnection($"Data Source={_dbPath}");
        _dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite($"Data Source={_dbPath}").Options);
    }

    [Fact]
    public async Task Cards_MapsUrlColumnAndAllScalarProperties()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var rowColumnId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO RowColumn (RowPosition, RowWidth) VALUES (1,1); SELECT last_insert_rowid();");
        await _connection.ExecuteAsync(
            "INSERT INTO Card (IndexPosition, RowColumnId, Name, Url, IconUrl, Type) VALUES (1, @row, 'Docs', 'https://example.test', '/icon.svg', 'link');",
            new { row = rowColumnId });

        var card = await _dbContext.Cards.SingleAsync();

        Assert.Equal("Docs", card.Name);
        Assert.Equal("https://example.test", card.Url);
        Assert.Equal("/icon.svg", card.IconUrl);
        Assert.Equal("link", card.Type);
        Assert.Equal((int)rowColumnId, card.RowColumnId);
    }

    [Fact]
    public async Task Settings_And_IconsConnected_And_Icons_MapCorrectly()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var rowColumnId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO RowColumn (RowPosition, RowWidth) VALUES (1,1); SELECT last_insert_rowid();");
        var cardId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO Card (IndexPosition, RowColumnId, Name, Url, IconUrl, Type) VALUES (1, @row, 'Docs', '', '', ''); SELECT last_insert_rowid();",
            new { row = rowColumnId });
        await _connection.ExecuteAsync(
            "INSERT INTO Settings (CardId, Width, Height, TitleHidden, ImageHidden) VALUES (@CardId, 400, 300, 1, 0);",
            new { CardId = cardId });
        var iconId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO Icon (Name, Type, Base64Data, MaterialIcon) VALUES ('grid', 'svg', 'Zm9v', 'dashboard'); SELECT last_insert_rowid();");
        await _connection.ExecuteAsync(
            "INSERT INTO IconsConnected (CardId, IconId) VALUES (@CardId, @IconId);",
            new { CardId = cardId, IconId = iconId });

        var settings = await _dbContext.Settings.SingleAsync();
        var iconsConnected = await _dbContext.IconsConnected.SingleAsync();
        var icon = await _dbContext.Icons.SingleAsync();

        Assert.Equal(400, settings.Width);
        Assert.Equal(300, settings.Height);
        Assert.True(settings.TitleHidden);
        Assert.Equal((int)cardId, iconsConnected.CardId);
        Assert.Equal((int)iconId, iconsConnected.IconId);
        Assert.Equal("grid", icon.Name);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        if (File.Exists(_dbPath + ".bak")) File.Delete(_dbPath + ".bak");
    }
}
