using System.Data;
using Dapper;
using Gridly.Data;
using Gridly.Models;
using Gridly.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace Gridly.Tests.Repositories;

public sealed class CardRepositoryTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-card-batchedit-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;
    private readonly GridlyDbContext _dbContext;

    public CardRepositoryTests()
    {
        _connection = new SqliteConnection($"Data Source={_dbPath}");
        _dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>()
                .UseSqlite($"Data Source={_dbPath}")
                .Options);
    }

    private async Task<(int cardId, int settingsId)> SeedCardAsync(int indexPosition, int rowColumnId, int width, int height)
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();

        var cardId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO Card (IndexPosition, RowColumnId, Name, Url, Type, IconUrl) VALUES (@indexPosition, @rowColumnId, 'A', '', 'Empty', ''); SELECT last_insert_rowid();",
            new { indexPosition, rowColumnId });
        var settingsId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO Settings (CardId, Width, Height, TitleHidden, ImageHidden) VALUES (@cardId, @width, @height, 0, 0); SELECT last_insert_rowid();",
            new { cardId, width, height });

        return ((int)cardId, (int)settingsId);
    }

    private async Task<int> SeedRowColumnAsync()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        return (int)await _connection.QuerySingleAsync<long>(
            "INSERT INTO RowColumn (RowPosition, RowWidth) VALUES (1,1); SELECT last_insert_rowid();");
    }

    [Fact]
    public async Task BatchEdit_WhenCardsIsNull_ReturnsFalse()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new CardRepository(_connection, _dbContext);

        var result = await repository.BatchEdit(null);

        Assert.False(result);
    }

    [Fact]
    public async Task BatchEdit_UpdatesIndexPositionRowColumnIdAndSettingsForEachCard()
    {
        var rowColumnId = await SeedRowColumnAsync();
        var otherRowColumnId = await SeedRowColumnAsync();
        var (card1Id, _) = await SeedCardAsync(indexPosition: 0, rowColumnId, width: 250, height: 250);
        var (card2Id, _) = await SeedCardAsync(indexPosition: 1, rowColumnId, width: 250, height: 250);
        var repository = new CardRepository(_connection, _dbContext);

        var result = await repository.BatchEdit(new List<CardModel>
        {
            new()
            {
                Id = card1Id, IndexPosition = 5, RowColumnId = otherRowColumnId,
                Settings = new SettingsModel { Width = 400, Height = 300 }
            },
            new()
            {
                Id = card2Id, IndexPosition = 6, RowColumnId = otherRowColumnId,
                Settings = new SettingsModel { Width = 500, Height = 350 }
            }
        });

        Assert.True(result);

        var card1 = await _connection.QuerySingleAsync(
            "SELECT IndexPosition, RowColumnId FROM Card WHERE Id = @card1Id", new { card1Id });
        Assert.Equal(5, (int)card1.IndexPosition);
        Assert.Equal(otherRowColumnId, (int)card1.RowColumnId);

        var settings1 = await _connection.QuerySingleAsync(
            "SELECT Width, Height FROM Settings WHERE CardId = @card1Id", new { card1Id });
        Assert.Equal(400, (int)settings1.Width);
        Assert.Equal(300, (int)settings1.Height);

        var settings2 = await _connection.QuerySingleAsync(
            "SELECT Width, Height FROM Settings WHERE CardId = @card2Id", new { card2Id });
        Assert.Equal(500, (int)settings2.Width);
        Assert.Equal(350, (int)settings2.Height);
    }

    [Fact]
    public async Task BatchEdit_WhenSettingsIsNull_DefaultsWidthAndHeightTo250()
    {
        var rowColumnId = await SeedRowColumnAsync();
        var (cardId, _) = await SeedCardAsync(indexPosition: 0, rowColumnId, width: 100, height: 100);
        var repository = new CardRepository(_connection, _dbContext);

        await repository.BatchEdit(new List<CardModel>
        {
            new() { Id = cardId, IndexPosition = 1, RowColumnId = rowColumnId, Settings = null }
        });

        var settings = await _connection.QuerySingleAsync(
            "SELECT Width, Height FROM Settings WHERE CardId = @cardId", new { cardId });
        Assert.Equal(250, (int)settings.Width);
        Assert.Equal(250, (int)settings.Height);
    }

    [Fact]
    public async Task BatchEdit_WhenCardDoesNotExist_ReturnsFalseAndChangesNothing()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new CardRepository(_connection, _dbContext);

        var result = await repository.BatchEdit(new List<CardModel>
        {
            new() { Id = 999, IndexPosition = 1, RowColumnId = 1, Settings = null }
        });

        Assert.False(result);
    }

    public void Dispose()
    {
        _connection.Dispose();
        _dbContext.Dispose();
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        if (File.Exists(_dbPath + ".bak")) File.Delete(_dbPath + ".bak");
    }
}
