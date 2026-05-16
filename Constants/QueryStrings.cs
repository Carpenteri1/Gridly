namespace Gridly.Constants;

public class QueryStrings
{
    public const string InsertToCardQuery = @"
    INSERT INTO Card (IndexPosition, Name, Url, IconUrl) 
    VALUES (@IndexPosition, @Name, @Url, @IconUrl);
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

    public const string SelectCardQuery = @"
     SELECT 
        co.Id AS CardId, 
        co.IndexPosition, 
        co.Name AS CardName, 
        co.Url, 
        co.IconUrl, 
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
        Url = @Url, 
        IconUrl = @IconUrl
        /**where**/";

    public const string UpdateBatchCardQuery = @"
    UPDATE Card 
    SET IndexPosition = @IndexPosition
    WHERE Id = @Id;

    UPDATE Settings 
    SET Width = @Width, Height = @Height
    WHERE CardId = @Id;
    ";

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

    public const string JoinIconDataQuery = "Icon i ON i.Id = ic.IconId";
    public const string JoinIconsConnectedDataQuery = "IconsConnected ic ON ic.CardId = co.Id";
    public const string JoinSettingsQuery = "Settings cs ON cs.CardId = co.Id";

    public const string WhereCardIdForeignKeyEqualId = "CardId = @CardId";
    public const string WhereIdEqualsId = "Id = @Id";
    public const string WhereIconConnectedIconIdForeignKeyEqualIdWithAlias = "ic.IconId = @IconId";
    public const string WhereIconConnectedCardIdForeignKeyEqualIdWithAlias = "ic.CardId = @CardId";
    public const string WhereCardIdEqualsCardIdWithAlias = "co.Id = @cardId";
    public const string WhereIconNameEqualsNameWithAlias = "i.Name = @Name";
    public const string WhereIconTypeEqualsTypeWithAlias = "i.Type = @Type";
    public const string IndexPositionWithAlias = "co.IndexPosition;";
}
