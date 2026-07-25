namespace Gridly.Services;

public interface IProviderKeysProtectionService
{
    public string Protect(string rawKey);
    public string Unprotect(string encryptedKey);
}
