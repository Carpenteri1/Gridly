using Gridly.Data;
using Gridly.Dtos;
using Gridly.Entities;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Tests.Repositories;

public sealed class WeatherRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly GridlyDbContext _dbContext;
    
    public WeatherRepositoryTests()
    {
        _connection = new SqliteConnection($"Data Source=:memory:");
        _connection.Open();
        _dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(_connection).Options);
        _dbContext.Database.EnsureCreated();
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
    public async Task Get_WhenAddressDoesNotExist_ReturnsNull()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new WeatherRepository(CreateDbContext());

        var result = await repository.Get("Nowhere");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetStoredWeatherData_WhenNoConnectionsExist_ReturnsEmpty()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new WeatherRepository(CreateDbContext());

        var result = await repository.GetStoredWeatherData();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetStoredWeatherData_WhenConnectionsExist_ReturnsJoinedRowsWithCorrectIds()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var dbContext = CreateDbContext();
        var repository = new WeatherRepository(dbContext);
        var (card1, _) = await SeedTwoCardsAsync();
        var weather = await repository.Insert(MakeWeather("Stockholm", "clear"));
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

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
    }
}
