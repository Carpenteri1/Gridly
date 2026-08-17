using Gridly.Data;
using Gridly.Tests.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Gridly.Tests.Services;

public sealed class DbConnectionServicesTests : IDisposable
{
    private readonly string _contentRoot =
        Path.Combine(Path.GetTempPath(), $"gridly-dbconnection-{Guid.NewGuid():N}");

    [Fact]
    public void Constructor_WhenConnectionStringIsMissing_ThrowsInvalidOperationException()
    {
        var configuration = BuildConfiguration(connectionString: null);
        var environment = new FakeHostEnvironment { ContentRootPath = _contentRoot };

        Assert.Throws<InvalidOperationException>(() => new DbConnectionServices(configuration, environment));
    }

    [Fact]
    public void Constructor_WhenConnectionStringIsValid_CreatesDatabaseDirectoryUnderContentRoot()
    {
        var configuration = BuildConfiguration(connectionString: "Data Source=data/gridly.db");
        var environment = new FakeHostEnvironment { ContentRootPath = _contentRoot };

        _ = new DbConnectionServices(configuration, environment);

        Assert.True(Directory.Exists(Path.Combine(_contentRoot, "data")));
    }

    [Fact]
    public void CreateConnection_ReturnsConnectionResolvedUnderContentRoot()
    {
        var configuration = BuildConfiguration(connectionString: "Data Source=data/gridly.db");
        var environment = new FakeHostEnvironment { ContentRootPath = _contentRoot };
        var service = new DbConnectionServices(configuration, environment);

        using var connection = service.CreateConnection();

        Assert.Equal(Path.Combine(_contentRoot, "data", "gridly.db"), connection.ConnectionString
            .Split(';')
            .Select(part => part.Split('=', 2))
            .First(part => part[0].Equals("Data Source", StringComparison.OrdinalIgnoreCase))[1]);
    }

    private static IConfiguration BuildConfiguration(string? connectionString)
    {
        var values = new Dictionary<string, string?>();
        if (connectionString is not null)
        {
            values["ConnectionStrings:GridlyDb"] = connectionString;
        }

        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }

    public void Dispose()
    {
        if (Directory.Exists(_contentRoot))
        {
            Directory.Delete(_contentRoot, recursive: true);
        }
    }
}
