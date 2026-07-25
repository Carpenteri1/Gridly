namespace Gridly.Services;

public interface IApiKeyProtectionService
{
    public string Protect(string rawKey);
    public string Unprotect(string encryptedKey);
}
