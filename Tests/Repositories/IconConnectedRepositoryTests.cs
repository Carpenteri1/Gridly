using System.Data;
using Dapper;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;

namespace Gridly.Tests.Repositories;

public sealed class IconConnectedRepositoryTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-iconsconnected-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;

    public IconConnectedRepositoryTests()
    {
        _connection = new SqliteConnection($"Data Source={_dbPath}");
    }

    private async Task<(int card1, int card2, int icon1, int icon2)> SeedFixtureAsync()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var rowColumnId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO RowColumn (RowPosition, RowWidth) VALUES (1,1); SELECT last_insert_rowid();");
        var card1 = await _connection.QuerySingleAsync<long>(
            "INSERT INTO Card (IndexPosition, RowColumnId, Name, Url, Type, IconUrl) VALUES (1, @row, 'A', '', '', ''); SELECT last_insert_rowid();",
            new { row = rowColumnId });
        var card2 = await _connection.QuerySingleAsync<long>(
            "INSERT INTO Card (IndexPosition, RowColumnId, Name, Url, Type, IconUrl) VALUES (2, @row, 'B', '', '', ''); SELECT last_insert_rowid();",
            new { row = rowColumnId });
        var icon1 = await _connection.QuerySingleAsync<long>(
            "INSERT INTO Icon (Name, Type, Base64Data, MaterialIcon) VALUES ('grid','svg','',''); SELECT last_insert_rowid();");
        var icon2 = await _connection.QuerySingleAsync<long>(
            "INSERT INTO Icon (Name, Type, Base64Data, MaterialIcon) VALUES ('box','svg','',''); SELECT last_insert_rowid();");
        return ((int)card1, (int)card2, (int)icon1, (int)icon2);
    }

    [Fact]
    public async Task Insert_CreatesConnectionAndReturnsGeneratedId()
    {
        var (card1, _, icon1, _) = await SeedFixtureAsync();
        var repository = new IconConnectedRepository(_connection);

        var result = await repository.Insert(new IconConnectedDtoModel { CardId = card1, IconId = icon1 });

        Assert.True(result.Id > 0);
        Assert.Equal(card1, result.CardId);
        Assert.Equal(icon1, result.IconId);
    }

    [Fact]
    public async Task GetManyById_FiltersByCardId()
    {
        var (card1, card2, icon1, _) = await SeedFixtureAsync();
        var repository = new IconConnectedRepository(_connection);
        await repository.Insert(new IconConnectedDtoModel { CardId = card1, IconId = icon1 });
        await repository.Insert(new IconConnectedDtoModel { CardId = card2, IconId = icon1 });

        var result = await repository.GetManyById(card1, null);

        var connection = Assert.Single(result);
        Assert.Equal(card1, connection.CardId);
    }

    [Fact]
    public async Task GetManyById_FiltersByIconId()
    {
        var (card1, card2, icon1, icon2) = await SeedFixtureAsync();
        var repository = new IconConnectedRepository(_connection);
        await repository.Insert(new IconConnectedDtoModel { CardId = card1, IconId = icon1 });
        await repository.Insert(new IconConnectedDtoModel { CardId = card2, IconId = icon2 });

        var result = await repository.GetManyById(null, icon1);

        var connection = Assert.Single(result);
        Assert.Equal(icon1, connection.IconId);
    }

    [Fact]
    public async Task GetManyById_WithNoFilters_ReturnsAllConnections()
    {
        var (card1, card2, icon1, icon2) = await SeedFixtureAsync();
        var repository = new IconConnectedRepository(_connection);
        await repository.Insert(new IconConnectedDtoModel { CardId = card1, IconId = icon1 });
        await repository.Insert(new IconConnectedDtoModel { CardId = card2, IconId = icon2 });

        var result = await repository.GetManyById(null, null);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Delete_RemovesConnectionsForCard()
    {
        var (card1, card2, icon1, icon2) = await SeedFixtureAsync();
        var repository = new IconConnectedRepository(_connection);
        await repository.Insert(new IconConnectedDtoModel { CardId = card1, IconId = icon1 });
        await repository.Insert(new IconConnectedDtoModel { CardId = card2, IconId = icon2 });

        var result = await repository.Delete(card1);

        Assert.True(result);
        Assert.Empty(await repository.GetManyById(card1, null));
        Assert.Single(await repository.GetManyById(card2, null));
    }

    public void Dispose()
    {
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        if (File.Exists(_dbPath + ".bak")) File.Delete(_dbPath + ".bak");
    }
}
