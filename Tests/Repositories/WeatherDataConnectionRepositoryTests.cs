using Gridly.Data;
using Gridly.Dtos;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Tests.Repositories;

public sealed class WeatherDataConnectionRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly GridlyDbContext _dbContext;
    private readonly WeatherDataConnectionRepository _repository;

    public WeatherDataConnectionRepositoryTests()
    {
        _connection.Open();
        _dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(_connection).Options);
        _dbContext.Database.EnsureCreated();
        _repository = new WeatherDataConnectionRepository(_dbContext);
    }

    [Fact]
    public async Task Upsert_WhenCardHasNoConnectionYet_CreatesOne()
    {
        var result = await _repository.Upsert(new WeatherDataConnectionDtoModel { CardId = 1, WeatherId = 10 });

        Assert.NotNull(result.Id);
        Assert.Equal(1, result.CardId);
        Assert.Equal(10, result.WeatherId);
        var connections = await _repository.GetManyById(1, null);
        Assert.Single(connections);
    }

    [Fact]
    public async Task Upsert_WhenCardAlreadyHasConnection_RepointsInsteadOfDuplicating()
    {
        await _repository.Upsert(new WeatherDataConnectionDtoModel { CardId = 1, WeatherId = 10 });

        await _repository.Upsert(new WeatherDataConnectionDtoModel { CardId = 1, WeatherId = 20 });

        var connections = (await _repository.GetManyById(1, null)).ToList();
        var connection = Assert.Single(connections);
        Assert.Equal(20, connection.WeatherId);
    }

    [Fact]
    public async Task GetManyById_WhenFilteringByCardId_ReturnsOnlyThatCardsConnection()
    {
        await _repository.Upsert(new WeatherDataConnectionDtoModel { CardId = 1, WeatherId = 10 });
        await _repository.Upsert(new WeatherDataConnectionDtoModel { CardId = 2, WeatherId = 20 });

        var connections = (await _repository.GetManyById(1, null)).ToList();

        var connection = Assert.Single(connections);
        Assert.Equal(1, connection.CardId);
        Assert.Equal(10, connection.WeatherId);
    }

    [Fact]
    public async Task GetManyById_WhenFilteringByWeatherId_ReturnsAllCardsSharingThatWeather()
    {
        await _repository.Upsert(new WeatherDataConnectionDtoModel { CardId = 1, WeatherId = 10 });
        await _repository.Upsert(new WeatherDataConnectionDtoModel { CardId = 2, WeatherId = 10 });

        var connections = await _repository.GetManyById(null, 10);

        Assert.Equal(2, connections.Count());
    }

    [Fact]
    public async Task Delete_WhenConnectionExists_RemovesOnlyTheGivenCardsConnectionAndReturnsTrue()
    {
        await _repository.Upsert(new WeatherDataConnectionDtoModel { CardId = 1, WeatherId = 10 });
        await _repository.Upsert(new WeatherDataConnectionDtoModel { CardId = 2, WeatherId = 10 });

        var result = await _repository.Delete(1);

        Assert.True(result);
        Assert.Empty(await _repository.GetManyById(1, null));
        Assert.Single(await _repository.GetManyById(2, null));
    }

    [Fact]
    public async Task Delete_WhenConnectionDoesNotExist_ReturnsFalse()
    {
        var result = await _repository.Delete(999);

        Assert.False(result);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
    }
}
