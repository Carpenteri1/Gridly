using System.Data;
using Gridly.Data;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;

namespace Gridly.Tests.Repositories;

public sealed class LocalProversRepositoryTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-localprovers-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;

    public LocalProversRepositoryTests()
    {
        _connection = new SqliteConnection($"Data Source={_dbPath}");
    }

    [Fact]
    public async Task Get_WhenProviderIsNotStored_ReturnsNull()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new LocalProversRepository(_connection);

        var result = await repository.Get("VisualCrossing");

        Assert.Null(result);
    }

    [Fact]
    public async Task Upsert_WhenProviderIsNew_InsertsKey()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new LocalProversRepository(_connection);

        var result = await repository.Upsert("VisualCrossing", "encrypted-key", "Unknown");

        Assert.True(result);
        var stored = await repository.Get("VisualCrossing");
        Assert.NotNull(stored);
        Assert.Equal("encrypted-key", stored.EncryptedKey);
        Assert.Equal("Unknown", stored.Status);
    }

    [Fact]
    public async Task Upsert_WhenProviderAlreadyExists_UpdatesInPlace()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new LocalProversRepository(_connection);
        await repository.Upsert("VisualCrossing", "first-key", "Unknown");

        await repository.Upsert("VisualCrossing", "second-key", "Valid");

        var stored = await repository.Get("VisualCrossing");
        Assert.NotNull(stored);
        Assert.Equal("second-key", stored.EncryptedKey);
        Assert.Equal("Valid", stored.Status);
    }

    [Fact]
    public async Task UpdateStatus_UpdatesStoredStatus()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new LocalProversRepository(_connection);
        await repository.Upsert("VisualCrossing", "encrypted-key", "Unknown");

        var result = await repository.UpdateStatus("VisualCrossing", "Invalid");

        Assert.True(result);
        var stored = await repository.Get("VisualCrossing");
        Assert.NotNull(stored);
        Assert.Equal("Invalid", stored.Status);
    }

    public void Dispose()
    {
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        if (File.Exists(_dbPath + ".bak")) File.Delete(_dbPath + ".bak");
    }
}
