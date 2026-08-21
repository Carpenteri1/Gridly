using Gridly.Data;
using Gridly.Entities;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Tests.Repositories;

public sealed class LocalProversRepositoryUpsertTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly GridlyDbContext _dbContext;
    private readonly LocalProversRepository _repository;

    public LocalProversRepositoryUpsertTests()
    {
        _connection.Open();
        _dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(_connection).Options);
        _dbContext.Database.EnsureCreated();
        _repository = new LocalProversRepository(_connection, _dbContext);
    }

    [Fact]
    public async Task Upsert_WhenProviderDoesNotExist_InsertsNewRowAndReturnsTrue()
    {
        var result = await _repository.Upsert("VisualCrossing", "encrypted-key", "Unknown");

        Assert.True(result);
        var persisted = await _dbContext.ProviderKeys.AsNoTracking().SingleAsync(p => p.Provider == "VisualCrossing");
        Assert.Equal("encrypted-key", persisted.EncryptedKey);
        Assert.Equal("Unknown", persisted.Status);
        Assert.NotNull(persisted.LastValidatedAt);
    }

    [Fact]
    public async Task Upsert_WhenProviderAlreadyExists_UpdatesInPlaceInsteadOfDuplicating()
    {
        await _repository.Upsert("VisualCrossing", "first-key", "Unknown");

        var result = await _repository.Upsert("VisualCrossing", "second-key", "Valid");

        Assert.True(result);
        var rows = await _dbContext.ProviderKeys.AsNoTracking().ToListAsync();
        var persisted = Assert.Single(rows);
        Assert.Equal("second-key", persisted.EncryptedKey);
        Assert.Equal("Valid", persisted.Status);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
    }
}

public sealed class LocalProversRepositoryProviderUniqueConstraintTests
{
    [Fact]
    public async Task ProviderUniqueConstraint_RejectsDirectDuplicateInsert()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connection).Options);
        await dbContext.Database.EnsureCreatedAsync();

        dbContext.ProviderKeys.Add(MakeProviderKey("VisualCrossing"));
        await dbContext.SaveChangesAsync();

        dbContext.ProviderKeys.Add(MakeProviderKey("VisualCrossing"));
        await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());
    }

    private static ProviderKeyEntity MakeProviderKey(string provider) =>
        new()
        {
            Provider = provider,
            EncryptedKey = "encrypted-key",
            Status = "Unknown",
            LastValidatedAt = DateTime.UtcNow,
        };
}
