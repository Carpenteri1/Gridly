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
    
    public const string SelectComponentQuery = @"
    SELECT co.Id, co.Name, co.Url, co.IconUrl, co.TitleHidden, co.ImageHidden, 
    cs.Width  AS Width, cs.Height AS Height, 
    i.name AS IconName, i.Type AS Type, i.Base64Data AS Base64Data 
    FROM Component co /**leftjoin**//**where**/";
    
    public const string UpdateComponentQuery = @"
    UPDATE Component co 
    SET co.Id = @Id, 
        co.Name = @Name, 
        co.Url = @Url, 
        co.IconUrl = @IconUrl,
        co.TitleHidden = @TitleHidden, 
        co.ImageHidden = @ImageHidden";
    
    public const string DeleteFromComponentSettingsQuery = "DELETE FROM ComponentSettings /**where**/";
    public const string DeleteFromComponentQuery = "DELETE FROM Component /**where**/";
    public const string DeleteFromIconQuery = "DELETE FROM Icon /**where**/";
    
    public const string JoinIconDataQuery = "Icon i ON i.ComponentId = co.Id";
    public const string JoinComponentSettingsQuery = "ComponentSettings cs ON cs.ComponentId = co.Id";
    
    public const string WhereComponentIdForeignKeyEqualComponentObjectId = "ComponentId = @ComponentId";
    public const string WhereComponentIdPrimaryKeyEqualComponentObjectId = "Id = @Id";
    public const string WhereComponentPrimaryKeyEqualsComponentSettingsForeignKey = "co.Id = cs.ComponentId";
    public const string WhereIconForeignKeyEqualsComponentPrimaryKey = "i.ComponentId = co.Id";
    
    public const string JoinIconData = "Icon i ON i.ComponentId = c.Id";
    public const string JoinComponentSettings = "ComponentSettings cs ON cs.ComponentId = c.Id";
    public const string WhereComponentSettingsData = "c.Id= cs.ComponentId";
    
    public const string WhereComponentIdEqualsComponentSettingsComponentIdQuery = "co.Id = cs.ComponentId";
    public const string WhereComponentSettingsComponentIdEqualsObjectIdQuery = "cs.ComponentId = @ComponentId";
    public const string WhereComponentIdEqualsObjectIdQuery = "co.Id = cs.ComponentId";
}