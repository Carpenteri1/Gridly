namespace Gridly.Constants;

public class QueryStrings
{
    public const string InsertToComponentQuery = @"
    INSERT INTO Component (Name, Url, IconUrl, TitleHidden, ImageHidden) 
    VALUES (@Name, @Url, @IconUrl, @TitleHidden, @ImageHidden);
    SELECT * FROM Component WHERE Id = last_insert_rowid();";
    
    public const string InsertToComponentSettingsQuery = @"
    INSERT INTO ComponentSettings (ComponentId, Width, Height) 
    VALUES (@ComponentId, @Width, @Height);
    SELECT * FROM ComponentSettings WHERE Id = last_insert_rowid();";
    
    public const string InsertToIconQuery = @"
    INSERT INTO Icon (Name, Type, Base64Data) 
    VALUES (@Name, @Type, @Base64Data);
    SELECT * FROM Icon WHERE Id = last_insert_rowid();";
    
    public const string InsertToConnectedIconQuery = @"
    INSERT INTO IconsConnected (ComponentId, IconId) 
    VALUES (@ComponentId, @IconId);
    SELECT * FROM IconsConnected WHERE Id = last_insert_rowid();";
    
    public const string SelectComponentQuery = @"
     SELECT 
        co.Id AS ComponentId, 
        co.Name AS ComponentName, 
        co.Url, 
        co.IconUrl, 
        co.ImageHidden, 
        co.TitleHidden,
        cs.Width AS Width,
        cs.Height AS Height,
        cs.Id AS ComponentSettingsId,
        i.Id AS IconId,
        i.Type AS Type,
        i.Name AS IconName,
        i.Base64Data AS Base64Data
        FROM Component co /**leftjoin**//**where**/";
    
    public const string SelectIconQuery = @"
    SELECT i.Id, i.Name, i.Type, i.Base64Data 
    FROM Icon i /**leftjoin**//**where**/";
    
    public const string SelectIconConnectedQuery = @"
    SELECT *
    FROM IconsConnected ic /**leftjoin**//**where**/";
    
    public const string UpdateComponentQuery = @"
    UPDATE Component
    SET Name = @Name, 
        Url = @Url, 
        IconUrl = @IconUrl,
        TitleHidden = @TitleHidden, 
        ImageHidden = @ImageHidden /**where**/";
    
    public const string UpdateComponentSettingsQuery = @"
    UPDATE ComponentSettings
    SET ComponentId = @Id
        Width = @Width
        Height = @Height /**where**/";
    
    public const string DeleteFromComponentSettingsQuery = "DELETE FROM ComponentSettings /**where**/";
    public const string DeleteFromComponentQuery = "DELETE FROM Component /**where**/";
    public const string DeleteFromIconsConnectedQuery = "DELETE FROM IconsConnected /**where**/";
    public const string DeleteFromIconQuery = "DELETE FROM Icon /**where**/";
    
    public const string JoinIconDataQuery = "Icon i ON i.Id = ic.IconId";
    public const string JoinIconsConnectedDataQuery = "IconsConnected ic ON ic.ComponentId = co.Id";
    public const string JoinComponentSettingsQuery = "ComponentSettings cs ON cs.ComponentId = co.Id";
    
    public const string WhereComponentIdForeignKeyEqualId = "ComponentId = @ComponentId";
    public const string WhereComponentIdPrimaryKeyEqualComponentObjectId = "Id = @Id";
    public const string WhereIconIdForeignKeyEqualId = "Id = @Id";
    
    public const string WhereIconConnectedIconIdForeignKeyEqualIdWithAlias = "ic.IconId = @IconId";
    public const string WhereIconConnectedComponentIdForeignKeyEqualIdWithAlias = "ic.ComponentId = @ComponentId";
    public const string WhereComponentForeignKeyEqualsComponentSettingsForeignKeyWithAlias = "co.Id = cs.ComponentId";
    public const string WhereComponentIdEqualsComponentIdWithAlias = "co.Id = @componentId";
    public const string WhereIconNameEqualsNameWithAlias = "i.Name = @Name";
    public const string WhereIconTypeEqualsTypeWithAlias = "i.Type = @Type";
}