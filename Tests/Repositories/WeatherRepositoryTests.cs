using System.Data;
using Gridly.Data;
using Gridly.Dtos;
using Gridly.Entities;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Tests.Repositories;

public sealed class WeatherRepositoryTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-weather-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;

    private sealed record WeatherRow(long Id, string Address, string Description);

    public WeatherRepositoryTests()
    {
        _connection = new SqliteConnection($"Data Source={_dbPath}");
    }

    private GridlyDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite((SqliteConnection)_connection).Options);

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
        var dbContext = CreateDbContext();
        var repository = new WeatherRepository(_connection, dbContext);

        var first = await repository.Upsert(MakeWeather("Stockholm", "clear"));
        var second = await repository.Upsert(MakeWeather("Stockholm", "cloudy"));

        Assert.Equal(first.Id, second.Id);
        var stored = await repository.Get("Stockholm");
        Assert.NotNull(stored);
        Assert.Equal("cloudy", stored.Description);
        var rowCount = await dbContext.WeatherData.CountAsync();
        Assert.Equal(1, rowCount);
    }

    [Fact]
    public async Task Get_WhenAddressDoesNotExist_ReturnsNull()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new WeatherRepository(_connection, CreateDbContext());

        var result = await repository.Get("Nowhere");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetStoredWeatherData_WhenNoConnectionsExist_ReturnsEmpty()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new WeatherRepository(_connection, CreateDbContext());

        var result = await repository.GetStoredWeatherData();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStoredWeatherData_WhenConnectionsExist_ReturnsJoinedRowsWithCorrectIds()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var dbContext = CreateDbContext();
        var repository = new WeatherRepository(_connection, dbContext);
        var (card1, _) = await SeedTwoCardsAsync();
        var weather = await repository.Upsert(MakeWeather("Stockholm", "clear"));
        dbContext.WeatherDataConnections.Add(new WeatherDataConnectionEntity { CardId = card1, WeatherId = weather.Id });
        await dbContext.SaveChangesAsync();

        var result = (await repository.GetStoredWeatherData())!.ToList();

        var single = Assert.Single(result);
        Assert.Equal(card1, single.CardId);
        Assert.Equal(weather.Id, single.Id);
        Assert.Equal("Stockholm", single.Address);
    }

    [Fact]
    public async Task AddressUniqueConstraint_RejectsDirectDuplicateInsert()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var dbContext = CreateDbContext();

        dbContext.WeatherData.Add(MakeWeatherEntity("Stockholm"));
        await dbContext.SaveChangesAsync();

        dbContext.WeatherData.Add(MakeWeatherEntity("Stockholm"));
        await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());
    }

    private static WeatherDataEntity MakeWeatherEntity(string address) =>
        new()
        {
            Address = address,
            Timezone = "Europe/Stockholm",
            Description = "clear",
            Temp = 1,
            FeelsLike = 1,
            Humidity = 1,
            WindSpeed = 1,
            WindDir = 1,
            FetchedAt = new DateTime(2026, 1, 1),
        };

    private async Task<(int card1, int card2)> SeedTwoCardsAsync()
    {
        var dbContext = CreateDbContext();
        var rowColumn = new RowColumnEntity { RowPosition = 1, RowWidth = 1 };
        dbContext.RowColumns.Add(rowColumn);
        await dbContext.SaveChangesAsync();

        var card1 = new CardEntity { IndexPosition = 1, RowColumnId = rowColumn.Id, Name = "A", Url = "", Type = "Weather", IconUrl = "" };
        var card2 = new CardEntity { IndexPosition = 2, RowColumnId = rowColumn.Id, Name = "B", Url = "", Type = "Weather", IconUrl = "" };
        dbContext.Cards.AddRange(card1, card2);
        await dbContext.SaveChangesAsync();

        return (card1.Id, card2.Id);
    }

    [Fact]
    public async Task EnsureTablesCreatedAsync_WhenLegacySchemaExists_MigratesDataIntoJunctionTableAndDropsCardId()
    {
        var dbContext = CreateDbContext();
        await dbContext.Database.ExecuteSqlRawAsync(@"
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

        var weatherRows = await dbContext.Database
            .SqlQueryRaw<WeatherRow>("SELECT Id, Address, Description FROM WeatherData;")
            .ToListAsync();
        Assert.Equal(2, weatherRows.Count);

        var stockholmRow = weatherRows.Single(r => r.Address == "Stockholm");
        Assert.Equal("cloudy", stockholmRow.Description);
        var stockholmWeatherId = (int)stockholmRow.Id;

        var connections = await dbContext.WeatherDataConnections.AsNoTracking().ToListAsync();
        Assert.Equal(3, connections.Count);
        Assert.Equal(2, connections.Count(c => c.WeatherId == stockholmWeatherId));
        Assert.Equal(3, connections.Select(c => c.CardId).Distinct().Count());

        var columnNames = await dbContext.Database
            .SqlQueryRaw<string>("SELECT name FROM pragma_table_info('WeatherData');")
            .ToListAsync();
        Assert.DoesNotContain("CardId", columnNames);

        Assert.True(File.Exists(_dbPath + ".bak"));
    }

    [Fact]
    public async Task EnsureTablesCreatedAsync_WhenRunTwice_IsIdempotent()
    {
        var dbContext = CreateDbContext();
        await dbContext.Database.ExecuteSqlRawAsync(@"
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

        var weatherRowCount = await dbContext.WeatherData.CountAsync();
        var connectionRowCount = await dbContext.WeatherDataConnections.CountAsync();
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
