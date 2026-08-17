using Gridly.Enums;

namespace Gridly.Models;
public class ProviderKeyModel
{
    public int Id { get; set; }
    public required string Provider { get; set; }
    public ProvidersKeyStatusEnum KeyStatus { get; set; }
    public DateTime? LastValidatedAt { get; set; }
}
