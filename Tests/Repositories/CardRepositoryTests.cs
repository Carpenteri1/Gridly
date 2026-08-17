using System.Data;
using Gridly.Data;
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
