using Gridly.Data;
using Gridly.Dtos;
using Gridly.Entities;
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
        var rowColumn = new ColumnRowEntity { RowPosition = 1, RowWidth = 1 };
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
