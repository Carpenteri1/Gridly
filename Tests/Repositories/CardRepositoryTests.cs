using System.Data;
using Dapper;
using Gridly.Data;
using Gridly.Models;
using Gridly.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace Gridly.Tests.Repositories;

public sealed class CardRepositoryTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-card-batchedit-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;
    private readonly GridlyDbContext _dbContext;

    public CardRepositoryTests()
    {
        _connection = new SqliteConnection($"Data Source={_dbPath}");
        _dbContext = new GridlyDbContext(
            new DbContextOptionsBuilder<GridlyDbContext>()
                .UseSqlite($"Data Source={_dbPath}")
                .Options);
    }

    public void Dispose()
    {
        _connection.Dispose();
        _dbContext.Dispose();
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        if (File.Exists(_dbPath + ".bak")) File.Delete(_dbPath + ".bak");
    }
}
