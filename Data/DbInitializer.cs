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
                CREATE TABLE IF NOT EXISTS Card(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                IndexPosition INTEGER NOT NULL,
                Name TEXT,
                URL TEXT,
                IconUrl TEXT);

                CREATE TABLE IF NOT EXISTS IconsConnected(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CardId INTEGER,
                IconId INTEGER,
                FOREIGN KEY (CardId) REFERENCES Card(Id) ON DELETE CASCADE,
                FOREIGN KEY (IconId) REFERENCES Icon(Id) ON DELETE CASCADE);

                CREATE TABLE IF NOT EXISTS Icon(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                Type TEXT,
                Base64Data TEXT,
                MaterialIcon TEXT);

                CREATE TABLE IF NOT EXISTS Settings(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CardId INTEGER NOT NULL,
                Width INT NOT NULL,
                Height INT NOT NULL,
                TitleHidden INTEGER,
                ImageHidden INTEGER,
                FOREIGN KEY(CardId) REFERENCES Card(Id));",
            commandTimeout:150);
    }
}