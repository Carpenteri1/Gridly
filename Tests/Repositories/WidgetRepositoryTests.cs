using System.Data;
using Gridly.Data;
using Gridly.Repositories;
using Microsoft.Data.Sqlite;

namespace Gridly.Tests.Repositories;

public sealed class WidgetRepositoryTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"gridly-widgets-{Guid.NewGuid():N}.db");
    private readonly IDbConnection _connection;

    public WidgetRepositoryTests()
    {
        _connection = new SqliteConnection($"Data Source={_dbPath}");
    }

    [Fact]
    public async Task Get_ReturnsSeededDefaultWidgetsWithJoinedWidgetTypeName()
    {
        await new DbInitializer(_connection).EnsureTablesCreatedAsync();
        var repository = new WidgetRepository(_connection);

        var result = (await repository.Get()).ToList();

        Assert.Equal(3, result.Count);
        var byType = result.ToDictionary(w => w.WidgetType);
        Assert.Equal("Weather widget", byType["Weather"].Label);
        Assert.Equal("clouds", byType["Weather"].Icon);
        Assert.Equal("Empty widget", byType["Empty"].Label);
        Assert.Equal("Custom widget", byType["Custom"].Label);
    }

    public void Dispose()
    {
        _connection.Dispose();
        SqliteConnection.ClearAllPools();
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        if (File.Exists(_dbPath + ".bak")) File.Delete(_dbPath + ".bak");
    }
}
