using System.Data;
using Dapper;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;

namespace Gridly.Tests.Repositories;

public sealed class WeatherDataConnectionRepositoryTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-weather-connections-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;

    public WeatherDataConnectionRepositoryTests()
    {
        _connection = new SqliteConnection($"Data Source={_dbPath}");
    }

    private async Task<(int card1, int card2, int weather1, int weather2)> SeedFixtureAsync()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();

        var rowColumnId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO RowColumn (RowPosition, RowWidth) VALUES (1,1); SELECT last_insert_rowid();");
        var cardAId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO Card (IndexPosition, RowColumnId, Name, Url, Type, IconUrl) VALUES (1, @row, 'A', '', 'Weather', ''); SELECT last_insert_rowid();",
            new { row = rowColumnId });
        var cardBId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO Card (IndexPosition, RowColumnId, Name, Url, Type, IconUrl) VALUES (2, @row, 'B', '', 'Weather', ''); SELECT last_insert_rowid();",
            new { row = rowColumnId });

        var weatherRepository = new WeatherRepository(_connection);
        var weather1 = await weatherRepository.Upsert(new()
        {
            Address = "Stockholm", Timezone = "Europe/Stockholm", Description = "clear",
            Temp = 20, FeelsLike = 20, Humidity = 50, WindSpeed = 5, WindDir = 180, FetchedAt = DateTime.UtcNow,
        });
        var weather2 = await weatherRepository.Upsert(new()
        {
            Address = "Gothenburg", Timezone = "Europe/Stockholm", Description = "rain",
            Temp = 15, FeelsLike = 14, Humidity = 80, WindSpeed = 8, WindDir = 200, FetchedAt = DateTime.UtcNow,
        });

        return ((int)cardAId, (int)cardBId, weather1.Id, weather2.Id);
    }

    [Fact]
    public async Task Upsert_WhenCardHasNoConnectionYet_CreatesOne()
    {
        var (card1, _, weather1, _) = await SeedFixtureAsync();
        var repository = new WeatherDataConnectionRepository(_connection);

        await repository.Upsert(new WeatherDataConnectionDtoModel { CardId = card1, WeatherId = weather1 });

        var connections = await repository.GetManyById(card1, null);
        Assert.Single(connections);
    }

    [Fact]
    public async Task CardIdUniqueConstraint_RepointsExistingConnectionInsteadOfDuplicating()
    {
        var (card1, _, weather1, weather2) = await SeedFixtureAsync();
        var repository = new WeatherDataConnectionRepository(_connection);
        await repository.Upsert(new WeatherDataConnectionDtoModel { CardId = card1, WeatherId = weather1 });

        await repository.Upsert(new WeatherDataConnectionDtoModel { CardId = card1, WeatherId = weather2 });

        var connections = (await repository.GetManyById(card1, null)).ToList();
        var connection = Assert.Single(connections);
        Assert.Equal(weather2, connection.WeatherId);
    }

    [Fact]
    public async Task GetManyById_WhenFilteringByWeatherId_ReturnsAllCardsSharingThatRow()
    {
        var (card1, card2, weather1, _) = await SeedFixtureAsync();
        var repository = new WeatherDataConnectionRepository(_connection);
        await repository.Upsert(new WeatherDataConnectionDtoModel { CardId = card1, WeatherId = weather1 });
        await repository.Upsert(new WeatherDataConnectionDtoModel { CardId = card2, WeatherId = weather1 });

        var connections = await repository.GetManyById(null, weather1);

        Assert.Equal(2, connections.Count());
    }

    [Fact]
    public async Task Delete_RemovesOnlyTheGivenCardsConnection()
    {
        var (card1, card2, weather1, _) = await SeedFixtureAsync();
        var repository = new WeatherDataConnectionRepository(_connection);
        await repository.Upsert(new WeatherDataConnectionDtoModel { CardId = card1, WeatherId = weather1 });
        await repository.Upsert(new WeatherDataConnectionDtoModel { CardId = card2, WeatherId = weather1 });

        await repository.Delete(card1);

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
