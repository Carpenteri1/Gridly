using System.Data;
using Microsoft.Data.Sqlite;

namespace Gridly.Data;

public class DbConnectionServices : IDbConnectionServices
{
    private readonly string connectionString;

    public DbConnectionServices(IConfiguration configuration, IHostEnvironment environment)
    {
        var configuredConnectionString = configuration.GetConnectionString("GridlyDb");
        if (string.IsNullOrWhiteSpace(configuredConnectionString))
        {
            throw new InvalidOperationException("Connection string 'GridlyDb' is missing.");
        }

        var sqliteBuilder = new SqliteConnectionStringBuilder(configuredConnectionString);
        if (!Path.IsPathRooted(sqliteBuilder.DataSource))
        {
            sqliteBuilder.DataSource = Path.Combine(environment.ContentRootPath, sqliteBuilder.DataSource);
        }

        var dbDirectory = Path.GetDirectoryName(sqliteBuilder.DataSource);
        if (!string.IsNullOrWhiteSpace(dbDirectory))
        {
            Directory.CreateDirectory(dbDirectory);
        }

        connectionString = sqliteBuilder.ConnectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(connectionString);
    }
}
