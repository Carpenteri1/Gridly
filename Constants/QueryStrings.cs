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
    
    public const string UpdateProviderKeyStatusQuery = @"
    UPDATE ProviderKeys
    SET Status = @Status,
        LastValidatedAt = @LastValidatedAt
    WHERE Provider = @Provider;";
    
    public const string WhereIdEqualsId = "Id = @Id";
}
