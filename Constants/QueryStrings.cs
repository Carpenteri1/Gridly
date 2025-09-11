namespace Gridly.Constants;

public class QueryStrings
{
    public const string InsertToComponentQuery = @"
    INSERT INTO Component (Name, Url, IconUrl, TitleHidden, ImageHidden) 
    VALUES (@Name, @Url, @IconUrl, @TitleHidden, @ImageHidden)";
    
    public const string InsertToComponentSettingsQuery = @"
    INSERT INTO ComponentSettings (ComponentId, Width, Height) 
    VALUES (@componentId, @width, @height)";
    
    public const string InsertToIconQuery = @"
    INSERT INTO Icon (ComponentId, Name, Type, Base64Data) 
    VALUES (@componentId, @name, @type, @base64Data)";
    
    public const string InsertToConnectedIconQuery = @"
    INSERT INTO IconsConnected (ComponentId, IconId) 
    VALUES (@ComponentId, @IconId)";
    
    public const string SelectComponentQuery = @"
    SELECT co.Id, co.Name, co.Url, co.IconUrl, co.TitleHidden, co.ImageHidden, 
    cs.Width  AS Width, cs.Height AS Height, 
    i.name AS IconName, i.Type AS Type, i.Base64Data AS Base64Data 
    FROM Component co /**leftjoin**//**where**/";
    
    public const string SelectComponentDataQuery = @"
    SELECT co.Id, co.Name, co.Url, co.IconUrl, co.TitleHidden, co.ImageHidden, 
    FROM Component co /**leftjoin**//**where**/";
    
    public const string SelectIconQuery = @"
    SELECT i.Id, i.ComponentId, i.Name, i.Type, i.Base64Data 
    FROM Icon i /**leftjoin**//**where**/";
    
    public const string SelectIconConnectedQuery = @"
    SELECT ic.Id, ic.ComponentId, ic.IconId
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
        Height = @Height
        /**where**/";
    
    public const string DeleteFromComponentSettingsQuery = "DELETE FROM ComponentSettings /**where**/";
    public const string DeleteFromComponentQuery = "DELETE FROM Component /**where**/";
    public const string DeleteFromIconsConnectedQuery = "DELETE FROM IconsConnected /**where**/";
    public const string DeleteFromIconQuery = "DELETE FROM Icon /**where**/";
    
    public const string JoinIconDataQuery = "Icon i ON i.ComponentId = co.Id";
    public const string JoinComponentSettingsQuery = "ComponentSettings cs ON cs.ComponentId = co.Id";
    
    public const string WhereComponentIdForeignKeyEqualId = "ComponentId = @ComponentId";
    public const string WhereComponentIdPrimaryKeyEqualComponentObjectId = "Id = @Id";
    public const string WhereIconIdForeignKeyEqualId = "IconId = @IconId";
    
    public const string WhereIconIdForeignKeyEqualIdWithAlias = "ic.IconId = @IconId";
    public const string WhereIconConnectedIconIdForeignKeyEqualWithAlias = "ic.IconId = @IconId";
    public const string WhereIconConnectedComponentIdForeignKeyEqualIdWithAlias = "ic.ComponentId = @ComponentId";
    public const string WhereComponentPrimaryKeyEqualsComponentSettingsForeignKeyWithAlias = "co.Id = cs.ComponentId";
    public const string WhereIconForeignKeyEqualsComponentPrimaryKeyWithAlias = "i.ComponentId = co.Id";
}