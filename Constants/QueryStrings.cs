namespace Gridly.Constants;

public class QueryStrings
{
    public const string InsertToRowQuery = @"
    INSERT INTO RowColumn (RowPosition, RowWidth) 
    VALUES (@RowPosition, @RowWidth);
    SELECT * FROM RowColumn WHERE Id = last_insert_rowid();";

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
    
    public const string UpdateCardQuery = @"
    UPDATE Card
    SET Name = @Name, 
        IndexPosition = @IndexPosition,
        RowColumnId = @RowColumnId,
        Url = @Url, 
        IconUrl = @IconUrl,
        Type = @Type
        /**where**/";
    
    public const string UpsertProviderKeyQuery = @"
    INSERT INTO ProviderKeys (Provider, EncryptedKey, Status, LastValidatedAt)
    VALUES (@Provider, @EncryptedKey, @Status, @LastValidatedAt)
    ON CONFLICT(Provider) DO UPDATE SET
        EncryptedKey = excluded.EncryptedKey,
        Status = excluded.Status,
        LastValidatedAt = excluded.LastValidatedAt;";

    public const string SelectWeatherDataQuery = @"
    SELECT Id, Address, Timezone, Description, Temp, FeelsLike, Humidity, WindSpeed, WindDir, FetchedAt
    FROM WeatherData /**where**/";

    public const string SelectAllWeatherDataQuery = @"
    SELECT wc.CardId, w.Id AS WeatherId, w.Address, w.Timezone, w.Description,
        w.Temp, w.FeelsLike, w.Humidity, w.WindSpeed, w.WindDir, w.FetchedAt
    FROM WeatherDataConnection wc
    INNER JOIN WeatherData w ON w.Id = wc.WeatherId /**where**/";

    public const string WhereLocationEqualsLocation = "Address = @Address";
    public const string WhereIdEqualsId = "Id = @Id";
}
