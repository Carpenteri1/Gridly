namespace Gridly.Constants;

public class QueryStrings
{
    public static readonly string InsertToComponent = @"
    INSERT INTO Component (Name, Url, IconUrl, TitleHidden, ImageHidden) 
    VALUES (@Name, @Url, @IconUrl, @TitleHidden, @ImageHidden)";
    
    public static readonly string InsertToComponentSettings = @"
    INSERT INTO ComponentSettings (ComponentId, Width, Height) 
    VALUES (@componentId, @width, @height)";
    
    public static readonly string InsertToIcon = @"
    INSERT INTO Icon (ComponentId, Name, Type, Base64Data) 
    VALUES (@componentId, @name, @type, @base64Data)";
    
    public static readonly string SelectComponentQuery = "SELECT * FROM Component c /**leftjoin**//**where**/";
    public static readonly string DeleteComponentQuery = "DELETE FROM Component c /**leftjoin**//**where**/";
    public static readonly string DeleteIconQuery = "DELETE FROM Icon i /**leftjoin**//**where**/";
    
    public static readonly string LeftJoinIconData = "Icon i ON i.ComponentId = c.Id";
    public static readonly string WhereIconData = "c.Id=@Id";
    public static readonly string WhereComponentData = "c.Id=@Id";
}