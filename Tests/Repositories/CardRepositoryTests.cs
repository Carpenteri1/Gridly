using System.Data;
using Dapper;
using Gridly.Data;
using Gridly.Models;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;

namespace Gridly.Tests.Repositories;

public sealed class CardRepositoryTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-cards-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;

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
        var repository = new CardRepository(_connection);

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
        var repository = new CardRepository(_connection);
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
        var stored = await repository.GetById(card.Id);
        Assert.NotNull(stored);
        Assert.Equal("Renamed", stored.Name);
        Assert.Equal("https://renamed.test", stored.Url);
    }

    [Fact]
    public async Task BatchEdit_UpdatesIndexPositionAndSettings()
    {
        var rowColumnId = await SeedRowColumnAsync();
        var repository = new CardRepository(_connection);
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
        var stored = await repository.GetById(card.Id);
        Assert.NotNull(stored);
        Assert.Equal(5, stored.IndexPosition);
        Assert.NotNull(stored.Settings);
        Assert.Equal(400, stored.Settings.Width);
        Assert.Equal(300, stored.Settings.Height);
    }

    [Fact]
    public async Task BatchEdit_WhenCardsIsNull_ReturnsFalse()
    {
        await SeedRowColumnAsync();
        var repository = new CardRepository(_connection);

        var result = await repository.BatchEdit(null);

        Assert.False(result);
    }

    [Fact]
    public async Task Get_ReturnsCardsOrderedByIndexPositionWithJoinedIconAndSettings()
    {
        var rowColumnId = await SeedRowColumnAsync();
        var iconId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO Icon (Name, Type, Base64Data, MaterialIcon) VALUES ('grid','svg','Zm9v','dashboard'); SELECT last_insert_rowid();");
        var repository = new CardRepository(_connection);
        var second = await repository.Insert(new CardModel { IndexPosition = 2, RowColumnId = rowColumnId, Name = "Second", Url = "", IconUrl = "", Type = "" });
        var first = await repository.Insert(new CardModel { IndexPosition = 1, RowColumnId = rowColumnId, Name = "First", Url = "", IconUrl = "", Type = "" });
        await _connection.ExecuteAsync(
            "INSERT INTO IconsConnected (CardId, IconId) VALUES (@CardId, @IconId);",
            new { CardId = first.Id, IconId = iconId });

        var result = (await repository.Get())!.ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("First", result[0].Name);
        Assert.Equal("Second", result[1].Name);
        Assert.Equal("grid", result[0].IconData!.Name);
    }

    [Fact]
    public async Task Delete_RemovesCard()
    {
        var rowColumnId = await SeedRowColumnAsync();
        var repository = new CardRepository(_connection);
        var card = await repository.Insert(new CardModel { IndexPosition = 1, RowColumnId = rowColumnId, Name = "ToDelete", Url = "", IconUrl = "", Type = "" });

        var result = await repository.Delete(card.Id);

        Assert.True(result);
        var remaining = await repository.Get();
        Assert.Empty(remaining!);
    }

    public void Dispose()
    {
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        if (File.Exists(_dbPath + ".bak")) File.Delete(_dbPath + ".bak");
    }
}
