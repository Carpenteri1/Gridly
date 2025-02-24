namespace Gridly.Models;

public class VersionModel
{
    public string tag_name { get; set; }
    public string created_at { get; set; }
    public bool newRelease { get; set; }
}