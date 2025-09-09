namespace Gridly.Constants;

public class QueryStrings
{
    public const string InsertToComponent = @"
    INSERT INTO Component (Name, Url, IconUrl, TitleHidden, ImageHidden) 
    VALUES (@Name, @Url, @IconUrl, @TitleHidden, @ImageHidden)";
    
    public const string InsertToComponentSettings = @"
    INSERT INTO ComponentSettings (ComponentId, Width, Height) 
    VALUES (@componentId, @width, @height)";
    
    public const string InsertToIcon = @"
    INSERT INTO Icon (ComponentId, Name, Type, Base64Data) 
    VALUES (@componentId, @name, @type, @base64Data)";
    
    public const string SelectComponentQuery = @"
    SELECT c.Id, c.Name, c.Url, c.IconUrl, c.TitleHidden, c.ImageHidden, 
    cs.Width  AS Width, cs.Height AS Height, 
    i.name AS IconName, i.Type AS Type, i.Base64Data AS Base64Data 
    FROM Component c /**leftjoin**/";
    
    public const string DeleteComponentQuery = "DELETE FROM Component c /**leftjoin**//**where**/";
    public const string DeleteIconQuery = "DELETE FROM Icon i /**leftjoin**//**where**/";
    
    public const string JoinIconData = "Icon i ON i.ComponentId = c.Id";
    public const string JoinComponentSettings = "ComponentSettings cs ON cs.ComponentId = c.Id";
    public const string WhereIconData = "i.ComponentId = c.Id";
    public const string WhereComponentSettingsData = "c.Id= cs.ComponentId";
}