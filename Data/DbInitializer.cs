using Dapper;
using System.Data;

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
                CardId INTEGER NOT NULL,
                Location TEXT NOT NULL UNIQUE,
                Address TEXT NOT NULL,
                Timezone TEXT NOT NULL,
                Description TEXT NOT NULL,
                Conditions TEXT NOT NULL,
                Temp REAL NOT NULL,
                FeelsLike REAL NOT NULL,
                Humidity REAL NOT NULL,
                WindSpeed REAL NOT NULL,
                WindDir REAL NOT NULL,
                FetchedAt TEXT NOT NULL,
                FOREIGN KEY(CardId) REFERENCES Card(Id));

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
    }
}
