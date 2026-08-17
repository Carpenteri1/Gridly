using System.Data;
using Dapper;
using Gridly.Data;
using Gridly.Entities;
using Gridly.Models;
using Gridly.Repositories;
using Gridly.Tests.Infrastructure;
using Microsoft.Data.Sqlite;

namespace Gridly.Tests.Repositories;

public sealed class CardRepositoryTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-cards-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;
    private readonly FakeGridlyDbContext _fakeDbContext = new();

    public CardRepositoryTests()
    {
        _connection = new SqliteConnection($"Data Source={_dbPath}");
    }

    private async Task<int> SeedRowColumnAsync()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var rowColumnId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO RowColumn (RowPosition, RowWidth) VALUES (1,1); SELECT last_insert_rowid();");
        return (int)rowColumnId;
    }

    [Fact]
    public async Task Insert_CreatesCardAndReturnsGeneratedId()
    {
        var rowColumnId = await SeedRowColumnAsync();
        var repository = new CardRepository(_connection, _fakeDbContext);

        var result = await repository.Insert(new CardModel
        {
            IndexPosition = 1,
            RowColumnId = rowColumnId,
            Name = "Docs",
            Url = "https://example.test",
            IconUrl = "/icons/docs.svg",
            Type = "",
        });

        Assert.True(result.Id > 0);
        Assert.Equal("Docs", result.Name);
    }

    [Fact]
    public async Task Edit_UpdatesNameAndUrl()
    {
        var rowColumnId = await SeedRowColumnAsync();
        var repository = new CardRepository(_connection, _fakeDbContext);
        var card = await repository.Insert(new CardModel
        {
            IndexPosition = 1,
            RowColumnId = rowColumnId,
            Name = "Original",
            Url = "https://original.test",
            IconUrl = "",
            Type = "",
        });

        card.Name = "Renamed";
        card.Url = "https://renamed.test";
        var edited = await repository.Edit(card);

        Assert.True(edited);
        var storedName = await _connection.QuerySingleAsync<string>(
            "SELECT Name FROM Card WHERE Id = @Id", new { card.Id });
        var storedUrl = await _connection.QuerySingleAsync<string>(
            "SELECT Url FROM Card WHERE Id = @Id", new { card.Id });
        Assert.Equal("Renamed", storedName);
        Assert.Equal("https://renamed.test", storedUrl);
    }

    [Fact]
    public async Task BatchEdit_UpdatesIndexPositionAndSettings()
    {
        var rowColumnId = await SeedRowColumnAsync();
        var repository = new CardRepository(_connection, _fakeDbContext);
        var card = await repository.Insert(new CardModel
        {
            IndexPosition = 1,
            RowColumnId = rowColumnId,
            Name = "Card",
            Url = "",
            IconUrl = "",
            Type = "",
        });
        await _connection.ExecuteAsync(
            "INSERT INTO Settings (CardId, Width, Height, TitleHidden, ImageHidden) VALUES (@CardId, 250, 250, 0, 0);",
            new { CardId = card.Id });

        var result = await repository.BatchEdit(new[]
        {
            new CardModel
            {
                Id = card.Id,
                IndexPosition = 5,
                RowColumnId = rowColumnId,
                Settings = new SettingsModel { Width = 400, Height = 300 },
            },
        });

        Assert.True(result);
        var storedIndexPosition = await _connection.QuerySingleAsync<int>(
            "SELECT IndexPosition FROM Card WHERE Id = @Id", new { card.Id });
        Assert.Equal(5, storedIndexPosition);
        var storedWidth = await _connection.QuerySingleAsync<int>(
            "SELECT Width FROM Settings WHERE CardId = @Id", new { card.Id });
        var storedHeight = await _connection.QuerySingleAsync<int>(
            "SELECT Height FROM Settings WHERE CardId = @Id", new { card.Id });
        Assert.Equal(400, storedWidth);
        Assert.Equal(300, storedHeight);
    }

    [Fact]
    public async Task BatchEdit_WhenCardsIsNull_ReturnsFalse()
    {
        await SeedRowColumnAsync();
        var repository = new CardRepository(_connection, _fakeDbContext);

        var result = await repository.BatchEdit(null);

        Assert.False(result);
    }

    [Fact]
    public async Task Delete_RemovesCard()
    {
        var rowColumnId = await SeedRowColumnAsync();
        var repository = new CardRepository(_connection, _fakeDbContext);
        var card = await repository.Insert(new CardModel
        {
            IndexPosition = 1, RowColumnId = rowColumnId, Name = "ToDelete", Url = "", IconUrl = "", Type = "",
        });

        var result = await repository.Delete(card.Id);

        Assert.True(result);
        var remaining = await _connection.QueryAsync("SELECT Id FROM Card WHERE Id = @Id", new { card.Id });
        Assert.Empty(remaining);
    }

    [Fact]
    public async Task Get_ReturnsCardsOrderedByIndexPositionWithJoinedIconAndSettings()
    {
        var repository = new CardRepository(_connection, _fakeDbContext);
        _fakeDbContext.CardEntities.Add(new CardEntity { Id = 1, IndexPosition = 1, RowColumnId = 1, Name = "First", Url = "", Type = "", IconUrl = "" });
        _fakeDbContext.CardEntities.Add(new CardEntity { Id = 2, IndexPosition = 2, RowColumnId = 1, Name = "Second", Url = "", Type = "", IconUrl = "" });
        _fakeDbContext.SettingsEntities.Add(new SettingsEntity { Id = 1, CardId = 1, Width = 250, Height = 250 });
        _fakeDbContext.SettingsEntities.Add(new SettingsEntity { Id = 2, CardId = 2, Width = 300, Height = 300 });
        _fakeDbContext.IconEntities.Add(new IconEntity { Id = 1, Name = "grid", Type = "svg", Base64Data = "Zm9v", MaterialIcon = "dashboard" });
        _fakeDbContext.IconEntities.Add(new IconEntity { Id = 2, Name = "other", Type = "svg", Base64Data = "", MaterialIcon = "box" });
        _fakeDbContext.IconsConnectedEntities.Add(new IconsConnectedEntity { Id = 1, CardId = 1, IconId = 1 });
        _fakeDbContext.IconsConnectedEntities.Add(new IconsConnectedEntity { Id = 2, CardId = 2, IconId = 2 });

        var result = (await repository.Get())!.ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("First", result[0].Name);
        Assert.Equal("Second", result[1].Name);
        Assert.Equal("grid", result[0].IconData!.Name);
        Assert.Equal(250, result[0].Settings!.Width);
    }

    public void Dispose()
    {
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        if (File.Exists(_dbPath + ".bak")) File.Delete(_dbPath + ".bak");
    }
}
