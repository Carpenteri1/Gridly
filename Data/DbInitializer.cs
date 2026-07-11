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

                INSERT INTO WidgetType(Name)
                SELECT 'Empty'
                WHERE NOT EXISTS (SELECT 1 FROM WidgetType WHERE Name = 'Empty');

                INSERT INTO WidgetType(Name)
                SELECT 'Custom'
                WHERE NOT EXISTS (SELECT 1 FROM WidgetType WHERE Name = 'Custom');

                INSERT INTO WidgetType(Name)
                SELECT 'Weather'
                WHERE NOT EXISTS (SELECT 1 FROM WidgetType WHERE Name = 'Weather');

                INSERT INTO Widget(WidgetType, Label, Description, Icon)
                SELECT Id, 'Add weather widget', '', 'bi bi-clouds-fill'
                FROM WidgetType 
                WHERE Name = 'Weather' 
                  AND NOT EXISTS (SELECT 1 FROM Widget WHERE Label = 'Add weather widget');

                INSERT INTO Widget(WidgetType, Label, Description, Icon)
                SELECT Id, 'Add empty widget', '', 'bi bi-box'
                FROM WidgetType
                WHERE Name = 'Empty'
                  AND NOT EXISTS (SELECT 1 FROM Widget WHERE Label = 'Add empty widget');

                INSERT INTO Widget(WidgetType, Label, Description, Icon)
                SELECT Id, 'Add custom widget', '', 'bi bi-box-fill'
                FROM WidgetType
                WHERE Name = 'Custom'
                  AND NOT EXISTS (SELECT 1 FROM Widget WHERE Label = 'Add custom widget');",
            commandTimeout:150);
    }
}
