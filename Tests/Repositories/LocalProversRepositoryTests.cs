using Gridly.Data;
using Gridly.Entities;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Tests.Repositories;

public sealed class LocalProversRepositoryTests
{
    [Fact]
    public async Task Get_WhenProviderExists_ReturnsMappedProviderKey()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connection).Options);
        await dbContext.Database.EnsureCreatedAsync();
        var repository = new LocalProversRepository(connection, dbContext);

        var lastValidatedAt = new DateTime(2026, 8, 21, 12, 0, 0, DateTimeKind.Utc);
        dbContext.ProviderKeys.Add(new ProviderKeyEntity
        {
            Provider = "VisualCrossing",
            EncryptedKey = "encrypted-value",
            Status = "Valid",
            LastValidatedAt = lastValidatedAt,
        });
        await dbContext.SaveChangesAsync();

        var result = await repository.Get("VisualCrossing");

        Assert.NotNull(result);
        Assert.Equal("VisualCrossing", result!.Provider);
        Assert.Equal("encrypted-value", result.EncryptedKey);
        Assert.Equal("Valid", result.Status);
        Assert.Equal(lastValidatedAt, result.LastValidatedAt);
    }

    [Fact]
    public async Task Get_WhenProviderDoesNotExist_ReturnsNull()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connection).Options);
        await dbContext.Database.EnsureCreatedAsync();
        var repository = new LocalProversRepository(connection, dbContext);

        var result = await repository.Get("Unknown");

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateStatus_WhenProviderExists_UpdatesStatusAndReturnsTrue()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connection).Options);
        await dbContext.Database.EnsureCreatedAsync();
        var repository = new LocalProversRepository(connection, dbContext);

        var entity = new ProviderKeyEntity
        {
            Provider = "VisualCrossing",
            EncryptedKey = "encrypted-value",
            Status = "Invalid",
            LastValidatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };
        dbContext.ProviderKeys.Add(entity);
        await dbContext.SaveChangesAsync();

        var result = await repository.UpdateStatus("VisualCrossing", "Valid");

        Assert.True(result);
        var persisted = await dbContext.ProviderKeys.AsNoTracking().SingleAsync(p => p.Id == entity.Id);
        Assert.Equal("Valid", persisted.Status);
        Assert.True(persisted.LastValidatedAt > new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public async Task UpdateStatus_WhenProviderDoesNotExist_ReturnsFalse()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connection).Options);
        await dbContext.Database.EnsureCreatedAsync();
        var repository = new LocalProversRepository(connection, dbContext);

        var result = await repository.UpdateStatus("Unknown", "Valid");

        Assert.False(result);
    }
}
