using System.Data;
using Dapper;
using Gridly.Data;
using Gridly.Models;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;

namespace Gridly.Tests.Repositories;

public sealed class ColumnRowRepositoryTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-columnrows-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;

    public ColumnRowRepositoryTests()
    {
        _connection = new SqliteConnection($"Data Source={_dbPath}");
    }

    [Fact]
    public async Task Insert_CreatesRowAndReturnsGeneratedId()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new ColumnRowRepository(_connection);

        var result = await repository.Insert(new ColumnRowModel { RowPosition = 1, RowWidth = 12, Cards = [] });

        Assert.True(result.Id > 0);
        Assert.Equal(1, result.RowPosition);
        Assert.Equal(12, result.RowWidth);
    }

    [Fact]
    public async Task Get_ReturnsRowsOrderedByRowPosition()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new ColumnRowRepository(_connection);
        await repository.Insert(new ColumnRowModel { RowPosition = 2, RowWidth = 12, Cards = [] });
        await repository.Insert(new ColumnRowModel { RowPosition = 1, RowWidth = 6, Cards = [] });

        var result = (await repository.Get())!.ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].RowPosition);
        Assert.Equal(2, result[1].RowPosition);
    }

    [Fact]
    public async Task BatchDelete_RemovesGivenRows()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new ColumnRowRepository(_connection);
        var row1 = await repository.Insert(new ColumnRowModel { RowPosition = 1, RowWidth = 12, Cards = [] });
        var row2 = await repository.Insert(new ColumnRowModel { RowPosition = 2, RowWidth = 12, Cards = [] });

        var result = await repository.BatchDelete(new[] { row1 });

        Assert.True(result);
        var remaining = (await repository.Get())!.ToList();
        Assert.Single(remaining);
        Assert.Equal(row2.Id, remaining[0].Id);
    }

    [Fact]
    public async Task BatchEdit_UpdatesRowPositionAndWidth()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new ColumnRowRepository(_connection);
        var row = await repository.Insert(new ColumnRowModel { RowPosition = 1, RowWidth = 12, Cards = [] });

        var result = await repository.BatchEdit(new[]
        {
            new ColumnRowModel { Id = row.Id, RowPosition = 3, RowWidth = 8, Cards = [] },
        });

        Assert.True(result);
        var stored = (await repository.Get())!.Single();
        Assert.Equal(3, stored.RowPosition);
        Assert.Equal(8, stored.RowWidth);
    }

    public void Dispose()
    {
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        if (File.Exists(_dbPath + ".bak")) File.Delete(_dbPath + ".bak");
    }
}
