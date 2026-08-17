using System.Data;
using System.Linq;
using Dapper;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;

namespace Gridly.Tests.Repositories;

public sealed class WeatherRepositoryTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-weather-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;

    public WeatherRepositoryTests()
    {
        _connection = new SqliteConnection($"Data Source={_dbPath}");
    }

    private static WeatherDataModel MakeWeather(string address, string description) =>
        new()
        {
            Address = address,
            Timezone = "Europe/Stockholm",
            Description = description,
            Temp = 20,
            FeelsLike = 20,
            Humidity = 50,
            WindSpeed = 5,
            WindDir = 180,
            FetchedAt = DateTime.UtcNow,
        };

    [Fact]
    public async Task Upsert_WhenAddressAlreadyExists_UpdatesInPlaceInsteadOfDuplicating()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new WeatherRepository(_connection);

        var first = await repository.Upsert(MakeWeather("Stockholm", "clear"));
        var second = await repository.Upsert(MakeWeather("Stockholm", "cloudy"));

        Assert.Equal(first.Id, second.Id);
        var stored = await repository.Get("Stockholm");
        Assert.Equal("cloudy", stored.Description);
        var rowCount = await _connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM WeatherData;");
        Assert.Equal(1, rowCount);
    }

    [Fact]
    public async Task AddressUniqueConstraint_RejectsDirectDuplicateInsert()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        const string insert = @"
            INSERT INTO WeatherData (Address, Timezone, Description, Temp, FeelsLike, Humidity, WindSpeed, WindDir, FetchedAt)
            VALUES ('Stockholm','Europe/Stockholm','clear',1,1,1,1,1,'2026-01-01');";

        await _connection.ExecuteAsync(insert);

        await Assert.ThrowsAsync<SqliteException>(() => _connection.ExecuteAsync(insert));
    }

    private async Task<(int card1, int card2)> SeedTwoCardsAsync()
    {
        var rowColumnId = await _connection.QuerySingleAsync<long>(
            "INSERT INTO RowColumn (RowPosition, RowWidth) VALUES (1,1); SELECT last_insert_rowid();");
        var card1 = await _connection.QuerySingleAsync<long>(
            "INSERT INTO Card (IndexPosition, RowColumnId, Name, Url, Type, IconUrl) VALUES (1, @row, 'A', '', 'Weather', ''); SELECT last_insert_rowid();",
            new { row = rowColumnId });
        var card2 = await _connection.QuerySingleAsync<long>(
            "INSERT INTO Card (IndexPosition, RowColumnId, Name, Url, Type, IconUrl) VALUES (2, @row, 'B', '', 'Weather', ''); SELECT last_insert_rowid();",
            new { row = rowColumnId });
        return ((int)card1, (int)card2);
    }

    [Fact]
    public async Task EnsureTablesCreatedAsync_WhenLegacySchemaExists_MigratesDataIntoJunctionTableAndDropsCardId()
    {
        await _connection.ExecuteAsync(@"
            CREATE TABLE RowColumn(Id INTEGER PRIMARY KEY AUTOINCREMENT, RowPosition INTEGER NOT NULL, RowWidth INTEGER NOT NULL);
            CREATE TABLE Card(Id INTEGER PRIMARY KEY AUTOINCREMENT, IndexPosition INTEGER NOT NULL, RowColumnId INTEGER NOT NULL, Name TEXT, URL TEXT, Type TEXT, IconUrl TEXT);
            CREATE TABLE WeatherData(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CardId INTEGER NOT NULL,
                Address TEXT NOT NULL,
                Timezone TEXT NOT NULL,
                Description TEXT NOT NULL,
                Temp REAL NOT NULL,
                FeelsLike REAL NOT NULL,
                Humidity REAL NOT NULL,
                WindSpeed REAL NOT NULL,
                WindDir REAL NOT NULL,
                FetchedAt TEXT NOT NULL,
                FOREIGN KEY(CardId) REFERENCES Card(Id));

            INSERT INTO RowColumn (RowPosition, RowWidth) VALUES (1,1);
            INSERT INTO Card (IndexPosition, RowColumnId, Name, Url, Type, IconUrl) VALUES
                (1,1,'A','','Weather',''),
                (1,1,'B','','Weather',''),
                (1,1,'C','','Weather','');

            INSERT INTO WeatherData (CardId, Address, Timezone, Description, Temp, FeelsLike, Humidity, WindSpeed, WindDir, FetchedAt) VALUES
                (1, 'Stockholm', 'Europe/Stockholm', 'clear',  20, 20, 50, 5, 180, '2026-01-01T10:00:00'),
                (2, 'Stockholm', 'Europe/Stockholm', 'cloudy', 18, 17, 60, 6, 190, '2026-01-02T10:00:00'),
                (3, 'Gothenburg', 'Europe/Stockholm', 'rain',  15, 14, 80, 8, 200, '2026-01-01T10:00:00');");

        await new DbInitializer(_connection).EnsureTablesCreatedAsync();

        var weatherRows = (await _connection.QueryAsync("SELECT Id, Address, Description FROM WeatherData;")).ToList();
        Assert.Equal(2, weatherRows.Count);

        var stockholmRow = weatherRows.Single(r => (string)r.Address == "Stockholm");
        Assert.Equal("cloudy", (string)stockholmRow.Description);
        var stockholmWeatherId = (int)(long)stockholmRow.Id;

        var connections = (await _connection.QueryAsync<WeatherDataConnectionDtoModel>("SELECT * FROM WeatherDataConnection;")).ToList();
        Assert.Equal(3, connections.Count);
        Assert.Equal(2, connections.Count(c => c.WeatherId == stockholmWeatherId));
        Assert.Equal(3, connections.Select(c => c.CardId).Distinct().Count());

        var columnNames = (await _connection.QueryAsync<string>("SELECT name FROM pragma_table_info('WeatherData');")).ToList();
        Assert.DoesNotContain("CardId", columnNames);

        Assert.True(File.Exists(_dbPath + ".bak"));
    }

    [Fact]
    public async Task EnsureTablesCreatedAsync_WhenRunTwice_IsIdempotent()
    {
        await _connection.ExecuteAsync(@"
            CREATE TABLE RowColumn(Id INTEGER PRIMARY KEY AUTOINCREMENT, RowPosition INTEGER NOT NULL, RowWidth INTEGER NOT NULL);
            CREATE TABLE Card(Id INTEGER PRIMARY KEY AUTOINCREMENT, IndexPosition INTEGER NOT NULL, RowColumnId INTEGER NOT NULL, Name TEXT, URL TEXT, Type TEXT, IconUrl TEXT);
            CREATE TABLE WeatherData(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CardId INTEGER NOT NULL,
                Address TEXT NOT NULL,
                Timezone TEXT NOT NULL,
                Description TEXT NOT NULL,
                Temp REAL NOT NULL,
                FeelsLike REAL NOT NULL,
                Humidity REAL NOT NULL,
                WindSpeed REAL NOT NULL,
                WindDir REAL NOT NULL,
                FetchedAt TEXT NOT NULL,
                FOREIGN KEY(CardId) REFERENCES Card(Id));

            INSERT INTO RowColumn (RowPosition, RowWidth) VALUES (1,1);
            INSERT INTO Card (IndexPosition, RowColumnId, Name, Url, Type, IconUrl) VALUES (1,1,'A','','Weather','');
            INSERT INTO WeatherData (CardId, Address, Timezone, Description, Temp, FeelsLike, Humidity, WindSpeed, WindDir, FetchedAt)
            VALUES (1, 'Stockholm', 'Europe/Stockholm', 'clear', 20, 20, 50, 5, 180, '2026-01-01T10:00:00');");

        var initializer = new DbInitializer(_connection);
        await initializer.EnsureTablesCreatedAsync();
        await initializer.EnsureTablesCreatedAsync();

        var weatherRowCount = await _connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM WeatherData;");
        var connectionRowCount = await _connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM WeatherDataConnection;");
        Assert.Equal(1, weatherRowCount);
        Assert.Equal(1, connectionRowCount);
    }

    public void Dispose()
    {
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        if (File.Exists(_dbPath + ".bak")) File.Delete(_dbPath + ".bak");
    }
}
