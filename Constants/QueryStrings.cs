namespace Gridly.Constants;

public class QueryStrings
{
    public const string InsertToRowQuery = @"
    INSERT INTO RowColumn (RowPosition, RowWidth) 
    VALUES (@RowPosition, @RowWidth);
    SELECT * FROM RowColumn WHERE Id = last_insert_rowid();";
    
    public const string InsertToCardQuery = @"
    INSERT INTO Card (RowColumnId, IndexPosition, Name, Url, IconUrl, Type) 
    VALUES (@RowColumnId, @IndexPosition, @Name, @Url, @IconUrl, @Type);
    SELECT * FROM Card WHERE Id = last_insert_rowid();";

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

    public const string InsertWeatherDataQuery = @"
    INSERT INTO WeatherData (CardId, Address, Timezone, Description, Temp, FeelsLike, Humidity, WindSpeed, WindDir, FetchedAt)
    VALUES (@CardId, @Address, @Timezone, @Description, @Temp, @FeelsLike, @Humidity, @WindSpeed, @WindDir, @FetchedAt);
    SELECT * FROM RowColumn WHERE Id = last_insert_rowid();";
    
    public const string SelectCardQuery = @"
     SELECT 
        co.Id AS CardId, 
        co.IndexPosition, 
        co.RowColumnId,
        co.Name AS CardName, 
        co.Url, 
        co.IconUrl, 
        co.Type AS CardType,
        cs.ImageHidden AS ImageHidden, 
        cs.TitleHidden AS TitleHidden,
        cs.Width AS Width,
        cs.Height AS Height,
        cs.Id AS SettingsId,
        i.Id AS IconId,
        i.Type AS Type,
        i.Name AS IconName,
        i.Base64Data AS Base64Data,
        i.MaterialIcon AS MaterialIcon
        FROM Card co /**leftjoin**//**where**//**orderby**/";

    public const string SelectRowQuery = @"
    SELECT r.Id AS Id, r.RowPosition AS RowPosition, r.RowWidth AS RowWidth
    FROM RowColumn r /**leftjoin**//**where**//**orderby**/";

    public const string SelectWidgetQuery = @"
    SELECT w.Id AS Id, wt.Name AS WidgetType, w.Label AS Label, w.Description AS Description, w.Icon AS Icon
    FROM Widget w /**leftjoin**//**where**//**orderby**/";
    
    public const string SelectIconQuery = @"
    SELECT i.Id, i.Name, i.Type, i.Base64Data, i.MaterialIcon 
    FROM Icon i /**leftjoin**//**where**/";

    public const string SelectIconConnectedQuery = @"
    SELECT *
    FROM IconsConnected ic /**leftjoin**//**where**/";

    public const string UpdateIconQuery = @"
    UPDATE Icon
    SET Name = @Name, 
        Type = @Type,
        Base64Data = @Base64Data, 
        MaterialIcon = @MaterialIcon
        /**where**/";

    public const string UpdateCardQuery = @"
    UPDATE Card
    SET Name = @Name, 
        IndexPosition = @IndexPosition,
        RowColumnId = @RowColumnId,
        Url = @Url, 
        IconUrl = @IconUrl,
        Type = @Type
        /**where**/";
    
    public const string UpdateBatchCardQuery = @"
    UPDATE Card 
    SET IndexPosition = @IndexPosition, 
        RowColumnId = @RowColumnId
    WHERE Id = @Id;

    UPDATE Settings 
    SET Width = @Width, Height = @Height
    WHERE CardId = @Id;
    ";

    public const string UpdateBatchRowColumnQuery = @"
    UPDATE RowColumn 
    SET RowPosition = @RowPosition, 
    RowWidth = @RowWidth
    WHERE Id = @Id;";
    
    public const string UpdateSettingsQuery = @"
    UPDATE Settings
    SET CardId = @Id,
        Width = @Width,
        Height = @Height,
        TitleHidden = @TitleHidden, 
        ImageHidden = @ImageHidden 
        /**where**/";

    public const string DeleteFromSettingsQuery = "DELETE FROM Settings /**where**/";
    public const string DeleteFromCardQuery = "DELETE FROM Card /**where**/";
    public const string DeleteFromIconsConnectedQuery = "DELETE FROM IconsConnected /**where**/";
    public const string DeleteFromIconQuery = "DELETE FROM Icon /**where**/";
    
    public const string BatchDeleteRowColumnQuery = @"
    DELETE FROM RowColumn
    WHERE Id IN @RowColumnId;";

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

    public const string UpdateWeatherDataQuery = @"
    UPDATE WeatherData
    SET Address = @Address,
        Timezone = @Timezone,
        Description = @Description,
        Temp = @Temp,
        FeelsLike = @FeelsLike,
        Humidity = @Humidity,
        WindSpeed = @WindSpeed,
        WindDir = @WindDir,
        FetchedAt = @FetchedAt
        WHERE CardId = @CardId;";
    
    public const string SelectWeatherDataQuery = @"
    SELECT Id, CardId, Address, Timezone, Description, Temp, FeelsLike, Humidity, WindSpeed, WindDir, FetchedAt
    FROM WeatherData /**where**/";

    public const string JoinIconDataQuery = "Icon i ON i.Id = ic.IconId";
    public const string JoinIconsConnectedDataQuery = "IconsConnected ic ON ic.CardId = co.Id";
    public const string JoinSettingsQuery = "Settings cs ON cs.CardId = co.Id";
    public const string JoinWidgetType = "WidgetType wt ON wt.Id = w.WidgetType";

    public const string WhereCardIdForeignKeyEqualId = "CardId = @CardId";
    public const string WhereLocationEqualsLocation = "Address = @Address";
    public const string WhereIdEqualsId = "Id = @Id";
    public const string WhereIconConnectedIconIdForeignKeyEqualIdWithAlias = "ic.IconId = @IconId";
    public const string WhereIconConnectedCardIdForeignKeyEqualIdWithAlias = "ic.CardId = @CardId";
    public const string WhereCardIdEqualsCardIdWithAlias = "co.Id = @cardId";
    public const string WhereIconNameEqualsNameWithAlias = "i.Name = @Name";
    public const string WhereIconTypeEqualsTypeWithAlias = "i.Type = @Type";
    public const string RowPositionWithAlias = "r.RowPosition;";
    public const string IndexPositionWithAlias = "co.IndexPosition;";
}
