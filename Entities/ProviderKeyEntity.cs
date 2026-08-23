namespace Gridly.Entities;

public class ProviderKeyEntity
{
    public int Id { get; set; }
    public string? Provider { get; set; }
    public string? EncryptedKey { get; set; }
    public string? Status { get; set; }
    public DateTime? LastValidatedAt { get; set; }
}
