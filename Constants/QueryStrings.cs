namespace Gridly.Constants;

public class QueryStrings
{ 
    public static readonly string InsertToComponent = @"
    INSERT INTO Component (Name, URL, IconUrl, TitleHidden, ImageHidden) 
    VALUES (@Name, @URL, @IconUrl, @TitleHidden, @ImageHidden)";
    
    public static readonly string InsertToIcon = @"
    INSERT INTO Icon (Name, Type, Base64) 
    VALUES (@Name, @Type, @Base64)";
    
    public static readonly string SelectComponentQuery = "SELECT * FROM Component c /**leftjoin**//**where**/";
    public static readonly string DeleteComponentQuery = "DELETE FROM Component c /**leftjoin**//**where**/";
    public static readonly string DeleteIconQuery = "DELETE FROM Icon i /**leftjoin**//**where**/";
    
    public static readonly string LeftJoinIconData = "Icon i ON i.ComponentId = c.Component.Id";
    public static readonly string WhereIconData = "c.Component.Id=@Id";
    public static readonly string WhereComponentData = "c.Component.Id=@Id";
}