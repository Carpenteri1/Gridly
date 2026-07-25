namespace Gridly.Models;

public static class ApiKeyStatus
{
    public const string Unknown = "Unknown";
    public const string Valid = "Valid";
    public const string Invalid = "Invalid";
}

public class ApiKeyModel
{
    public int Id { get; set; }
    public string Provider { get; set; }
    public string Status { get; set; }
    public DateTime? LastValidatedAt { get; set; }
}
