using System.Data;
using Gridly.Data;
using Gridly.Entities;
using Gridly.Models;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Gridly.Tests.Repositories;

public sealed class ColumnRowRepositoryGetTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-columnrow-get-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;
    private readonly GridlyDbContext _dbContext;
    private readonly ColumnRowRepository _repository;

    public ColumnRowRepositoryGetTests()
    {
        var connectionString = $"Data Source={_dbPath}";
        _connection = new SqliteConnection(connectionString);
        _dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connectionString).Options);
        _repository = new ColumnRowRepository(_connection, _dbContext);
    }

    [Fact]
    public async Task Get_WhenNoRowsExist_ReturnsEmptyCollection()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();

        var result = await _repository.Get();

        Assert.NotNull(result);
        Assert.Empty(result!);
    }

    [Fact]
    public async Task Get_WhenRowsExist_ReturnsMappedRowsOrderedByPosition()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(connection).Options);
        await dbContext.Database.EnsureCreatedAsync();
        var repository = new ColumnRowRepository(connection, dbContext);

        var second = new RowColumnEntity { RowPosition = 2, RowWidth = 6 };
        var first = new RowColumnEntity { RowPosition = 1, RowWidth = 12 };
        dbContext.RowColumns.AddRange(second, first);
        await dbContext.SaveChangesAsync();

        var result = (await repository.Get())!.ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(first.Id, result[0].Id);
        Assert.Equal(1, result[0].RowPosition);
        Assert.Equal(12, result[0].RowWidth);
        Assert.Empty(result[0].Cards);
        Assert.Equal(second.Id, result[1].Id);
        Assert.Equal(2, result[1].RowPosition);
        Assert.Equal(6, result[1].RowWidth);
        Assert.Empty(result[1].Cards);
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

public sealed class ColumnRowRepositoryBatchDeleteTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly GridlyDbContext _dbContext;
    private readonly ColumnRowRepository _repository;

    public ColumnRowRepositoryBatchDeleteTests()
    {
        _connection.Open();
        _dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>().UseSqlite(_connection).Options);
        _dbContext.Database.EnsureCreated();
        _repository = new ColumnRowRepository(_connection, _dbContext);
    }

    [Fact]
    public async Task BatchDelete_WhenRowsMatch_RemovesOnlyThoseRowsAndReturnsTrue()
    {
        var toDelete = new RowColumnEntity { RowPosition = 1, RowWidth = 12 };
        var toKeep = new RowColumnEntity { RowPosition = 2, RowWidth = 6 };
        _dbContext.RowColumns.AddRange(toDelete, toKeep);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.BatchDelete([new ColumnRowModel { Id = toDelete.Id, RowPosition = 1, RowWidth = 12, Cards = [] }]);

        Assert.True(result);
        var remaining = await _dbContext.RowColumns.AsNoTracking().ToListAsync();
        Assert.Single(remaining);
        Assert.Equal(toKeep.Id, remaining[0].Id);
    }

    [Fact]
    public async Task BatchDelete_WhenNoRowsMatch_ReturnsFalse()
    {
        var existing = new RowColumnEntity { RowPosition = 1, RowWidth = 12 };
        _dbContext.RowColumns.Add(existing);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.BatchDelete([new ColumnRowModel { Id = existing.Id + 1, RowPosition = 1, RowWidth = 12, Cards = [] }]);

        Assert.False(result);
        Assert.Single(await _dbContext.RowColumns.AsNoTracking().ToListAsync());
    }

    [Fact]
    public async Task BatchDelete_WhenColumnRowsIsNull_ReturnsFalse()
    {
        var result = await _repository.BatchDelete(null!);

        Assert.False(result);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }
}
