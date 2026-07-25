using Microsoft.AspNetCore.DataProtection;

namespace Gridly.Services;

public class ApiKeyProtectionService : IApiKeyProtectionService
{
    private const string Purpose = "ThirdPartyApiKey.v1";
    private readonly IDataProtector _protector;

    public ApiKeyProtectionService(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector(Purpose);
    }

    public string Protect(string rawKey) => _protector.Protect(rawKey);
    public string Unprotect(string encryptedKey) => _protector.Unprotect(encryptedKey);
}
