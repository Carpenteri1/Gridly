using System.Data;
using Gridly.Data;
using Gridly.Entities;
using Gridly.Models;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Tests.Repositories;

public sealed class CardRepositoryTests
{
    private static CardRepository CreateRepository(GridlyDbContext dbContext, IDbConnection connection) =>
        new(connection, dbContext);

    private static FakeGridlyDbContext CreateFakeDbContext(int saveChangesResult) =>
        new(new DbContextOptionsBuilder<GridlyDbContext>().Options, saveChangesResult);

    [Fact]
    public async Task BatchEdit_WhenCardsIsNull_ReturnsFalseAndNeverCallsSaveChanges()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        using var dbContext = CreateFakeDbContext(saveChangesResult: 1);
        var repository = CreateRepository(dbContext, connection);

        var result = await repository.BatchEdit(null);

        Assert.False(result);
        Assert.False(dbContext.SaveChangesCalled);
    }

    [Fact]
    public async Task BatchEdit_WhenBatchSaveSucceeds_ReturnsTrue()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        using var dbContext = CreateFakeDbContext(saveChangesResult: 1);
        var repository = CreateRepository(dbContext, connection);

        var result = await repository.BatchEdit(new List<CardModel> { new() { Id = 1 } });

        Assert.True(result);
    }

    [Fact]
    public async Task BatchEdit_WhenBatchSaveFails_ReturnsFalse()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        using var dbContext = CreateFakeDbContext(saveChangesResult: 0);
        var repository = CreateRepository(dbContext, connection);

        var result = await repository.BatchEdit(new List<CardModel> { new() { Id = 1 } });

        Assert.False(result);
    }

    private sealed class FakeGridlyDbContext(DbContextOptions<GridlyDbContext> options, int saveChangesResult)
        : GridlyDbContext(options)
    {
        public bool SaveChangesCalled { get; private set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCalled = true;
            return Task.FromResult(saveChangesResult);
        }
    }
}

public sealed class CardRepositoryGetTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-card-get-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;
    private readonly GridlyDbContext _dbContext;
    private readonly CardRepository _repository;

    public CardRepositoryGetTests()
    {
        var connectionString = $"Data Source={_dbPath}";
        _connection = new SqliteConnection(connectionString);
        _dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connectionString).Options);
        _repository = new CardRepository(_connection, _dbContext);
    }

    [Fact]
    public async Task Get_WhenNoCardsExist_ReturnsEmptyCollection()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();

        var result = await _repository.Get();

        Assert.NotNull(result);
        Assert.Empty(result!);
    }

    [Fact]
    public async Task Get_WhenCardsExist_ReturnsMappedCards()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connection).Options);
        await dbContext.Database.EnsureCreatedAsync();
        var repository = new CardRepository(connection, dbContext);

        var icon = new IconEntity
        {
            Name = "grid",
            Type = "svg",
            Base64Data = "Zm9v",
            MaterialIcon = "dashboard",
        };
        var cardEntity = new CardEntity
        {
            IndexPosition = 1,
            RowColumnId = 1,
            Name = "Docs",
            Url = "https://example.test",
            IconUrl = "/icon.svg",
            Type = "link",
        };
        cardEntity.Settings.Add(new SettingsEntity { Width = 400, Height = 300, TitleHidden = true, ImageHidden = false });
        cardEntity.IconsConnected.Add(new IconsConnectedEntity { Icon = icon });

        dbContext.Cards.Add(cardEntity);
        await dbContext.SaveChangesAsync();

        var result = await repository.Get();

        var card = Assert.Single(result!);
        Assert.Equal(cardEntity.Id, card.Id);
        Assert.Equal("Docs", card.Name);
        Assert.Equal("https://example.test", card.Url);
        Assert.Equal("/icon.svg", card.IconUrl);
        Assert.Equal("link", card.Type);
        Assert.NotNull(card.Settings);
        Assert.Equal(400, card.Settings!.Width);
        Assert.Equal(300, card.Settings.Height);
        Assert.True(card.Settings.TitleHidden);
        Assert.False(card.Settings.ImageHidden);
        Assert.NotNull(card.IconData);
        Assert.Equal(icon.Id, card.IconData!.Id);
        Assert.Equal("grid", card.IconData.Name);
        Assert.Equal("dashboard", card.IconData.MaterialIcon);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        if (File.Exists(_dbPath + ".bak")) File.Delete(_dbPath + ".bak");
    }
}