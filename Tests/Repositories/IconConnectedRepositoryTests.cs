using Gridly.Data;
using Gridly.Dtos;
using Gridly.Entities;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Tests.Repositories;

public sealed class IconConnectedRepositoryInsertTests
{
    [Fact]
    public async Task Insert_WhenModelIsValid_PersistsRowAndReturnsModelWithGeneratedId()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connection).Options);
        await dbContext.Database.EnsureCreatedAsync();
        var repository = new IconConnectedRepository(dbContext);

        var card = new CardEntity { Name = "card" };
        var icon = new IconEntity { Name = "icon" };
        dbContext.Cards.Add(card);
        dbContext.Icons.Add(icon);
        await dbContext.SaveChangesAsync();

        var model = new IconConnectedDtoModel { CardId = card.Id, IconId = icon.Id };

        var result = await repository.Insert(model);

        Assert.NotNull(result.Id);
        Assert.NotEqual(0, result.Id);
        Assert.Equal(card.Id, result.CardId);
        Assert.Equal(icon.Id, result.IconId);

        var persisted = await dbContext.IconsConnected.SingleAsync(ic => ic.Id == result.Id);
        Assert.Equal(card.Id, persisted.CardId);
        Assert.Equal(icon.Id, persisted.IconId);
    }
}

public sealed class IconConnectedRepositoryGetManyByIdTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly GridlyDbContext _dbContext;
    private readonly IconConnectedRepository _repository;
    private readonly int _cardOneId;
    private readonly int _cardTwoId;
    private readonly int _iconOneId;
    private readonly int _iconTwoId;

    public IconConnectedRepositoryGetManyByIdTests()
    {
        _connection.Open();
        _dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(_connection).Options);
        _dbContext.Database.EnsureCreated();
        _repository = new IconConnectedRepository(_dbContext);

        var cardOne = new CardEntity { Name = "card-one" };
        var cardTwo = new CardEntity { Name = "card-two" };
        var iconOne = new IconEntity { Name = "icon-ten" };
        var iconTwenty = new IconEntity { Name = "icon-twenty" };
        _dbContext.Cards.AddRange(cardOne, cardTwo);
        _dbContext.Icons.AddRange(iconOne, iconTwenty);
        _dbContext.SaveChanges();

            _cardOneId = cardOne.Id;
            _cardTwoId = cardTwo.Id;
            _iconOneId = iconOne.Id;
            _iconTwoId = iconTwenty.Id;

            _dbContext.IconsConnected.AddRange(
                new IconsConnectedEntity { CardId = _cardOneId, IconId = _iconOneId },
                new IconsConnectedEntity { CardId = _cardTwoId, IconId = _iconTwoId });
            _dbContext.SaveChanges();
    }

    [Fact]
    public async Task GetManyById_WhenOnlyCardIdProvided_ReturnsMatchingRows()
    {
        var result = await _repository.GetManyById(_cardOneId, null);

        var single = Assert.Single(result);
        Assert.Equal(_cardOneId, single.CardId);
    }

    [Fact]
    public async Task GetManyById_WhenOnlyIconIdProvided_ReturnsMatchingRows()
    {
        var result = await _repository.GetManyById(null, _iconOneId);
        
        var single = Assert.Single(result);
        Assert.Equal(_cardOneId, single.CardId);
    }

    [Fact]
    public async Task GetManyById_WhenBothIdsProvided_ReturnsSingleMatchingRow()
    {
        var result = await _repository.GetManyById(_cardTwoId, _iconOneId);

        var single = Assert.Single(result);
        Assert.Equal(_cardTwoId, single.CardId);
        Assert.Equal(_iconOneId, single.IconId);
    }

    [Fact]
    public async Task GetManyById_WhenNeitherIdProvided_ReturnsAllRows()
    {
        var result = await _repository.GetManyById(null, null);

        Assert.Equal(2, result.Count());
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
    }
}

public sealed class IconConnectedRepositoryDeleteTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly GridlyDbContext _dbContext;
    private readonly IconConnectedRepository _repository;

    public IconConnectedRepositoryDeleteTests()
    {
        _connection.Open();
        _dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(_connection).Options);
        _dbContext.Database.EnsureCreated();
        _repository = new IconConnectedRepository(_dbContext);
    }

    [Fact]
    public async Task Delete_WhenCardIdExists_RemovesMatchingRowsAndReturnsTrue()
    {
        var cardToDelete = new CardEntity { Name = "delete-me" };
        var cardToKeep = new CardEntity { Name = "keep-me" };
        var icon = new IconEntity { Name = "icon" };
        _dbContext.Cards.AddRange(cardToDelete, cardToKeep);
        _dbContext.Icons.Add(icon);
        await _dbContext.SaveChangesAsync();

        var toDelete = new IconsConnectedEntity { CardId = cardToDelete.Id, IconId = icon.Id };
        var toKeep = new IconsConnectedEntity { CardId = cardToKeep.Id, IconId = icon.Id };
        _dbContext.IconsConnected.AddRange(toDelete, toKeep);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.Delete(cardToDelete.Id);

        Assert.True(result);
        var remaining = await _dbContext.IconsConnected.AsNoTracking().ToListAsync();
        Assert.Single(remaining);
        Assert.Equal(toKeep.Id, remaining[0].Id);
    }

    [Fact]
    public async Task Delete_WhenCardIdDoesNotExist_ReturnsFalse()
    {
        var card = new CardEntity { Name = "keep-me" };
        var icon = new IconEntity { Name = "icon" };
        _dbContext.Cards.Add(card);
        _dbContext.Icons.Add(icon);
        await _dbContext.SaveChangesAsync();

        var existing = new IconsConnectedEntity { CardId = card.Id, IconId = icon.Id };
        _dbContext.IconsConnected.Add(existing);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.Delete(card.Id + 999);

        Assert.False(result);
        Assert.Single(await _dbContext.IconsConnected.AsNoTracking().ToListAsync());
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
    }
}
