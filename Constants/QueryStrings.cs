namespace Gridly.Constants;

public class QueryStrings
{
    public const string InsertToComponentQuery = @"
    INSERT INTO Component (IndexPosition, Name, Url, IconUrl) 
    VALUES (@IndexPosition, @Name, @Url, @IconUrl);
    SELECT * FROM Component WHERE Id = last_insert_rowid();";
    
    public const string InsertToComponentSettingsQuery = @"
    INSERT INTO ComponentSettings (ComponentId, Width, Height, TitleHidden, ImageHidden) 
    VALUES (@ComponentId, @Width, @Height, @TitleHidden, @ImageHidden);
    SELECT * FROM ComponentSettings WHERE Id = last_insert_rowid();";
    
    public const string InsertToIconQuery = @"
    INSERT INTO Icon (Name, Type, Base64Data, MaterialIcon) 
    VALUES (@Name, @Type, @Base64Data, @MaterialIcon);
    SELECT * FROM Icon WHERE Id = last_insert_rowid();";
    
    public const string InsertToConnectedIconQuery = @"
    INSERT INTO IconsConnected (ComponentId, IconId) 
    VALUES (@ComponentId, @IconId);
    SELECT * FROM IconsConnected WHERE Id = last_insert_rowid();";
    
    public const string SelectComponentQuery = @"
     SELECT 
        co.Id AS ComponentId, 
        co.IndexPosition, 
        co.Name AS ComponentName, 
        co.Url, 
        co.IconUrl, 
        cs.ImageHidden AS ImageHidden, 
        cs.TitleHidden AS TitleHidden,
        cs.Width AS Width,
        cs.Height AS Height,
        cs.Id AS ComponentSettingsId,
        i.Id AS IconId,
        i.Type AS Type,
        i.Name AS IconName,
        i.Base64Data AS Base64Data,
        i.MaterialIcon AS MaterialIcon
        FROM Component co /**leftjoin**//**where**//**orderby**/";
    
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

    public const string UpdateComponentQuery = @"
    UPDATE Component
    SET Name = @Name, 
        IndexPosition = @IndexPosition,
        Url = @Url, 
        IconUrl = @IconUrl
        /**where**/";

    public const string UpdateBatchComponentQuery = @"
    UPDATE Component 
    SET IndexPosition = @IndexPosition
    WHERE Id = @Id;

    UPDATE ComponentSettings 
    SET Width = @Width, Height = @Height
    WHERE ComponentId = @Id;
    ";
    
    public const string UpdateComponentSettingsQuery = @"
    UPDATE ComponentSettings
    SET ComponentId = @Id,
        Width = @Width,
        Height = @Height,
        TitleHidden = @TitleHidden, 
        ImageHidden = @ImageHidden 
        /**where**/";
    
    public const string DeleteFromComponentSettingsQuery = "DELETE FROM ComponentSettings /**where**/";
    public const string DeleteFromComponentQuery = "DELETE FROM Component /**where**/";
    public const string DeleteFromIconsConnectedQuery = "DELETE FROM IconsConnected /**where**/";
    public const string DeleteFromIconQuery = "DELETE FROM Icon /**where**/";
    
    public const string JoinIconDataQuery = "Icon i ON i.Id = ic.IconId";
    public const string JoinIconsConnectedDataQuery = "IconsConnected ic ON ic.ComponentId = co.Id";
    public const string JoinComponentSettingsQuery = "ComponentSettings cs ON cs.ComponentId = co.Id";
    
    public const string WhereComponentIdForeignKeyEqualId = "ComponentId = @ComponentId";
    public const string WhereIdEqualsId = "Id = @Id";
    public const string WhereIconConnectedIconIdForeignKeyEqualIdWithAlias = "ic.IconId = @IconId";
    public const string WhereIconConnectedComponentIdForeignKeyEqualIdWithAlias = "ic.ComponentId = @ComponentId";
    public const string WhereComponentIdEqualsComponentIdWithAlias = "co.Id = @componentId";
    public const string WhereIconNameEqualsNameWithAlias = "i.Name = @Name";
    public const string WhereIconTypeEqualsTypeWithAlias = "i.Type = @Type";
    public const string IndexPositionWithAlias = "co.IndexPosition;";
}
