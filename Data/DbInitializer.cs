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
                ComponentId INT PRIMARY KEY,
                Name TEXT NOT NULL,
                URL TEXT NOT NULL,
                IconUrl TEXT NOT NULL,
                TitleHidden BOOLEAN NOT NULL,
                ImageHidden BOOLEAN NOT NULL);

                CREATE TABLE IF NOT EXISTS Icon(
                IconId INT PRIMARY KEY,
                Name TEXT NOT NULL,
                Type TEXT NOT NULL,
                Base64 TEXT NOT NULL,
                FOREIGN KEY(IconId) REFERENCES Component(ComponentId));

                CREATE TABLE IF NOT EXISTS ComponentSettings(
                ComponentSettingsId INT PRIMARY KEY,
                Width INT NOT NULL,
                Height INT NOT NULL,
                FOREIGN KEY(ComponentSettingsId) REFERENCES Component(ComponentId));

                CREATE TABLE IF NOT EXISTS Version(
                    VersionId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    NewRelease BOOLEAN NOT NULL);",
            commandTimeout:150);
    }
}