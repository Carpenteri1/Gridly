namespace Gridly.Models;

public class VersionCheckStateModel
{
    public VersionModel Version { get; set; }
    public DateTime CheckedAtUtc { get; set; }
}
