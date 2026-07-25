using Microsoft.AspNetCore.DataProtection;

namespace Gridly.Services;

public class ProviderKeysProtectionService : IProviderKeysProtectionService
{
    private const string Purpose = "ProviderKeys.v1";
    private readonly IDataProtector _protector;

    public ProviderKeysProtectionService(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector(Purpose);
    }

    public string Protect(string rawKey) => _protector.Protect(rawKey);
    public string Unprotect(string encryptedKey) => _protector.Unprotect(encryptedKey);
}
