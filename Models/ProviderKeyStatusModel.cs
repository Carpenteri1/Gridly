using Gridly.Enums;

namespace Gridly.Models;

public class ProviderKeyStatusModel
{
    public bool Exists { get; set; }
    public ProvidersKeyStatusEnum KeyStatus { get; set; }
}
