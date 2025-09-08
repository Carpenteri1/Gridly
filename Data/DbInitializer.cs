using Dapper;
using Microsoft.Data.Sqlite;

namespace Gridly.Data;

public class DbInitializer
{
    private SqliteConnection connection;

    public DbInitializer(IConfiguration configuration)
    {
        connection = new SqliteConnection(configuration.GetConnectionString("GridlyDb"));
    }
    
    public async Task EnsureTablesCreatedAsync()
    {
        await connection.ExecuteAsync(
            sql:@"
                CREATE TABLE IF NOT EXISTS Component(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                URL TEXT NOT NULL,
                IconUrl TEXT NOT NULL,
                TitleHidden BOOLEAN NOT NULL,
                ImageHidden BOOLEAN NOT NULL);

                CREATE TABLE IF NOT EXISTS Icon(
                IconId INTEGER PRIMARY KEY AUTOINCREMENT,
                ComponentId INTEGER NOT NULL,
                Name TEXT NOT NULL,
                Type TEXT NOT NULL,
                Base64Data TEXT NOT NULL,
                FOREIGN KEY(ComponentId) REFERENCES Component(Id));

                CREATE TABLE IF NOT EXISTS ComponentSettings(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ComponentId INTEGER NOT NULL,
                Width INT NOT NULL,
                Height INT NOT NULL,
                FOREIGN KEY(ComponentId) REFERENCES Component(Id));

                CREATE TABLE IF NOT EXISTS LatestVersion(
                    Id INTEGER PRIMARY KEY,
                    Name TEXT NOT NULL,
                    NewRelease BOOLEAN NOT NULL);",
            commandTimeout:150);
    }
}