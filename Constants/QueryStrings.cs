namespace Gridly.Constants;

public class QueryStrings
{
    public const string InsertToRowQuery = @"
    INSERT INTO RowColumn (RowPosition, RowWidth) 
    VALUES (@RowPosition, @RowWidth);
    SELECT * FROM RowColumn WHERE Id = last_insert_rowid();";
    
    public const string InsertToSettingsQuery = @"
    INSERT INTO Settings (CardId, Width, Height, TitleHidden, ImageHidden) 
    VALUES (@CardId, @Width, @Height, @TitleHidden, @ImageHidden);
    SELECT * FROM Settings WHERE Id = last_insert_rowid();";

    public const string InsertToIconQuery = @"
    INSERT INTO Icon (Name, Type, Base64Data, MaterialIcon) 
    VALUES (@Name, @Type, @Base64Data, @MaterialIcon);
    SELECT * FROM Icon WHERE Id = last_insert_rowid();";

    public const string InsertToConnectedIconQuery = @"
    INSERT INTO IconsConnected (CardId, IconId) 
    VALUES (@CardId, @IconId);
    SELECT * FROM IconsConnected WHERE Id = last_insert_rowid();";

    public const string UpsertWeatherDataQuery = @"
    INSERT INTO WeatherData (Address, Timezone, Description, Temp, FeelsLike, Humidity, WindSpeed, WindDir, FetchedAt)
    VALUES (@Address, @Timezone, @Description, @Temp, @FeelsLike, @Humidity, @WindSpeed, @WindDir, @FetchedAt)
    ON CONFLICT(Address) DO UPDATE SET
        Timezone = excluded.Timezone,
        Description = excluded.Description,
        Temp = excluded.Temp,
        FeelsLike = excluded.FeelsLike,
        Humidity = excluded.Humidity,
        WindSpeed = excluded.WindSpeed,
        WindDir = excluded.WindDir,
        FetchedAt = excluded.FetchedAt
    RETURNING *;";

    public const string UpsertWeatherDataConnectionQuery = @"
    INSERT INTO WeatherDataConnection (CardId, WeatherId)
    VALUES (@CardId, @WeatherId)
    ON CONFLICT(CardId) DO UPDATE SET
        WeatherId = excluded.WeatherId
    RETURNING *;";
    
    public const string SelectWidgetQuery = @"
    SELECT w.Id AS Id, wt.Name AS WidgetType, w.Label AS Label, w.Description AS Description, w.Icon AS Icon
    FROM Widget w /**leftjoin**//**where**//**orderby**/";
    
    public const string SelectIconQuery = @"
    SELECT i.Id, i.Name, i.Type, i.Base64Data, i.MaterialIcon 
    FROM Icon i /**leftjoin**//**where**/";

    public const string SelectIconConnectedQuery = @"
    SELECT *
    FROM IconsConnected ic /**leftjoin**//**where**/";

    public const string UpdateCardQuery = @"
    UPDATE Card
    SET Name = @Name, 
        IndexPosition = @IndexPosition,
        RowColumnId = @RowColumnId,
        Url = @Url, 
        IconUrl = @IconUrl,
        Type = @Type
        /**where**/";
    
    public const string UpdateSettingsQuery = @"
    UPDATE Settings
    SET CardId = @Id,
        Width = @Width,
        Height = @Height,
        TitleHidden = @TitleHidden, 
        ImageHidden = @ImageHidden 
        /**where**/";

    public const string DeleteFromSettingsQuery = "DELETE FROM Settings /**where**/";
    public const string DeleteFromIconsConnectedQuery = "DELETE FROM IconsConnected /**where**/";

    public const string UpsertProviderKeyQuery = @"
    INSERT INTO ProviderKeys (Provider, EncryptedKey, Status, LastValidatedAt)
    VALUES (@Provider, @EncryptedKey, @Status, @LastValidatedAt)
    ON CONFLICT(Provider) DO UPDATE SET
        EncryptedKey = excluded.EncryptedKey,
        Status = excluded.Status,
        LastValidatedAt = excluded.LastValidatedAt;";

    public const string UpdateProviderKeyStatusQuery = @"
    UPDATE ProviderKeys
    SET Status = @Status,
        LastValidatedAt = @LastValidatedAt
    WHERE Provider = @Provider;";

    public const string SelectProviderKeyQuery = @"
    SELECT Id, Provider, EncryptedKey, Status, LastValidatedAt
    FROM ProviderKeys
    WHERE Provider = @Provider;";

    public const string SelectWeatherDataQuery = @"
    SELECT Id, Address, Timezone, Description, Temp, FeelsLike, Humidity, WindSpeed, WindDir, FetchedAt
    FROM WeatherData /**where**/";

    public const string SelectAllWeatherDataQuery = @"
    SELECT wc.CardId, w.Id AS WeatherId, w.Address, w.Timezone, w.Description,
        w.Temp, w.FeelsLike, w.Humidity, w.WindSpeed, w.WindDir, w.FetchedAt
    FROM WeatherDataConnection wc
    INNER JOIN WeatherData w ON w.Id = wc.WeatherId /**where**/";

    public const string SelectWeatherDataConnectionQuery = @"
    SELECT *
    FROM WeatherDataConnection wc /**where**/";

    public const string DeleteFromWeatherDataConnectionQuery = "DELETE FROM WeatherDataConnection /**where**/";

    public const string JoinWidgetType = "WidgetType wt ON wt.Id = w.WidgetType";

    public const string WhereCardIdForeignKeyEqualId = "CardId = @CardId";
    public const string WhereLocationEqualsLocation = "Address = @Address";
    public const string WhereIdEqualsId = "Id = @Id";
    public const string WhereWeatherConnectedCardIdForeignKeyEqualIdWithAlias = "wc.CardId = @CardId";
    public const string WhereWeatherConnectedWeatherIdForeignKeyEqualIdWithAlias = "wc.WeatherId = @WeatherId";
    public const string WhereIconConnectedIconIdForeignKeyEqualIdWithAlias = "ic.IconId = @IconId";
    public const string WhereIconConnectedCardIdForeignKeyEqualIdWithAlias = "ic.CardId = @CardId";
    public const string WhereIconNameEqualsNameWithAlias = "i.Name = @Name";
    public const string WhereIconTypeEqualsTypeWithAlias = "i.Type = @Type";
}
