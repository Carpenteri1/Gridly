namespace Gridly.Models;
public class ProviderKeyModel
{
    public int Id { get; set; }
    public string Provider { get; set; }
    public string Status { get; set; }
    public DateTime? LastValidatedAt { get; set; }
}
