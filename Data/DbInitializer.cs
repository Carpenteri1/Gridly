using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;

namespace Gridly.Data;

public class DbInitializer
{
    private readonly IDbConnection connection;

    public DbInitializer(IDbConnection connection)
    {
        this.connection = connection;
    }

    public async Task EnsureTablesCreatedAsync()
    {
        await connection.ExecuteAsync(
            sql:@"
                CREATE TABLE IF NOT EXISTS RowColumn(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                RowPosition INTEGER NOT NULL,
                RowWidth INTEGER NOT NULL);

                CREATE TABLE IF NOT EXISTS Card(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                IndexPosition INTEGER NOT NULL,
                RowColumnId INTEGER NOT NULL,
                Name TEXT,
                URL TEXT,
                Type TEXT,
                IconUrl TEXT,
                FOREIGN KEY(RowColumnId) REFERENCES RowColumn(Id) ON DELETE CASCADE);

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
                FOREIGN KEY(CardId) REFERENCES Card(Id));

                 CREATE TABLE IF NOT EXISTS WidgetType(
                 Id INTEGER PRIMARY KEY AUTOINCREMENT,
                 Name TEXT NOT NULL);

                 CREATE TABLE IF NOT EXISTS Widget(
                 Id INTEGER PRIMARY KEY AUTOINCREMENT,
                 WidgetType INTEGER,
                 Label TEXT NOT NULL,
                 Description TEXT NOT NULL,
                 Icon TEXT NOT NULL,
                 FOREIGN KEY(WidgetType) REFERENCES WidgetType(Id));

                CREATE TABLE IF NOT EXISTS ProviderKeys(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Provider TEXT NOT NULL UNIQUE,
                EncryptedKey TEXT NOT NULL,
                Status TEXT NOT NULL,
                LastValidatedAt TEXT);

                CREATE TABLE IF NOT EXISTS WeatherData(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Address TEXT NOT NULL UNIQUE,
                Timezone TEXT NOT NULL,
                Description TEXT NOT NULL,
                Temp REAL NOT NULL,
                FeelsLike REAL NOT NULL,
                Humidity REAL NOT NULL,
                WindSpeed REAL NOT NULL,
                WindDir REAL NOT NULL,
                FetchedAt TEXT NOT NULL);

                CREATE TABLE IF NOT EXISTS WeatherDataConnection(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CardId INTEGER NOT NULL,
                WeatherId INTEGER NOT NULL,
                UNIQUE(CardId),
                FOREIGN KEY(CardId) REFERENCES Card(Id) ON DELETE CASCADE,
                FOREIGN KEY(WeatherId) REFERENCES WeatherData(Id) ON DELETE CASCADE);

                CREATE INDEX IF NOT EXISTS idx_weatherdataconnection_weatherid ON WeatherDataConnection(WeatherId);

                INSERT INTO WidgetType(Name)
                SELECT 'Empty'
                WHERE NOT EXISTS (SELECT 1 FROM WidgetType WHERE Id = 1);

                INSERT INTO WidgetType(Name)
                SELECT 'Custom'
                WHERE NOT EXISTS (SELECT 1 FROM WidgetType WHERE Id = 2);

                INSERT INTO WidgetType(Name)
                SELECT 'Weather'
                WHERE NOT EXISTS (SELECT 1 FROM WidgetType WHERE Id = 3);

                INSERT INTO Widget(WidgetType, Label, Description, Icon)
                SELECT Id, 'Weather widget', '', 'clouds'
                FROM WidgetType
                WHERE Name = 'Weather'
                  AND NOT EXISTS (SELECT 1 FROM Widget WHERE Id = 1);

                INSERT INTO Widget(WidgetType, Label, Description, Icon)
                SELECT Id, 'Empty widget', '', 'box'
                FROM WidgetType
                WHERE Name = 'Empty'
                  AND NOT EXISTS (SELECT 1 FROM Widget WHERE Id = 2);

                INSERT INTO Widget(WidgetType, Label, Description, Icon)
                SELECT Id, 'Custom widget', '', 'box_add'
                FROM WidgetType
                WHERE Name = 'Custom'
                  AND NOT EXISTS (SELECT 1 FROM Widget WHERE Id = 3);",
            commandTimeout:150);

        await MigrateLegacyWeatherDataAsync();
    }

    private async Task MigrateLegacyWeatherDataAsync()
    {
        var columnNames = await connection.QueryAsync<string>(
            "SELECT name FROM pragma_table_info('WeatherData');");

        if (!columnNames.Contains("CardId"))
            return;

        BackupDatabaseFile();

        await connection.ExecuteAsync(
            sql: @"
                PRAGMA foreign_keys=OFF;
                BEGIN TRANSACTION;

                CREATE TABLE WeatherData_new(
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Address TEXT NOT NULL UNIQUE,
                Timezone TEXT NOT NULL,
                Description TEXT NOT NULL,
                Temp REAL NOT NULL,
                FeelsLike REAL NOT NULL,
                Humidity REAL NOT NULL,
                WindSpeed REAL NOT NULL,
                WindDir REAL NOT NULL,
                FetchedAt TEXT NOT NULL);

                INSERT INTO WeatherData_new (Address, Timezone, Description, Temp, FeelsLike, Humidity, WindSpeed, WindDir, FetchedAt)
                SELECT Address, Timezone, Description, Temp, FeelsLike, Humidity, WindSpeed, WindDir, FetchedAt
                FROM WeatherData wd
                WHERE wd.Id = (
                    SELECT wd2.Id FROM WeatherData wd2
                    WHERE wd2.Address = wd.Address
                    ORDER BY wd2.FetchedAt DESC, wd2.Id DESC
                    LIMIT 1
                );

                INSERT INTO WeatherDataConnection (CardId, WeatherId)
                SELECT legacy.CardId, wn.Id
                FROM (
                    SELECT wd.CardId, wd.Address
                    FROM WeatherData wd
                    WHERE wd.Id = (
                        SELECT wd2.Id FROM WeatherData wd2
                        WHERE wd2.CardId = wd.CardId
                        ORDER BY wd2.FetchedAt DESC, wd2.Id DESC
                        LIMIT 1
                    )
                ) legacy
                INNER JOIN WeatherData_new wn ON wn.Address = legacy.Address;

                DROP TABLE WeatherData;
                ALTER TABLE WeatherData_new RENAME TO WeatherData;

                COMMIT;
                PRAGMA foreign_keys=ON;",
            commandTimeout: 150);
    }

    private void BackupDatabaseFile()
    {
        var dataSource = new SqliteConnectionStringBuilder(connection.ConnectionString).DataSource;
        if (string.IsNullOrWhiteSpace(dataSource) || !File.Exists(dataSource))
            return;

        File.Copy(dataSource, dataSource + ".bak", overwrite: true);
    }
}
