namespace Gridly.Dtos;

public class ProviderKeyDtoModel
{
    public int Id { get; set; }
    public required string Provider { get; set; }
    public required string EncryptedKey { get; set; }
    public required string Status { get; set; }
    public DateTime? LastValidatedAt { get; set; }
}
