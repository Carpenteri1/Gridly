using Gridly.Services;
using Microsoft.AspNetCore.DataProtection;

namespace Gridly.Tests.Services;

public sealed class ProviderKeysProtectionServiceTests : IDisposable
{
    private readonly string _keyRingDirectory =
        Path.Combine(Path.GetTempPath(), $"gridly-dataprotection-{Guid.NewGuid():N}");
    private readonly ProviderKeysProtectionService _service;

    public ProviderKeysProtectionServiceTests()
    {
        Directory.CreateDirectory(_keyRingDirectory);
        var provider = DataProtectionProvider.Create(new DirectoryInfo(_keyRingDirectory));
        _service = new ProviderKeysProtectionService(provider);
    }

    [Fact]
    public void Protect_ThenUnprotect_RoundTripsToOriginalValue()
    {
        const string rawKey = "super-secret-api-key";

        var protectedValue = _service.Protect(rawKey);
        var unprotectedValue = _service.Unprotect(protectedValue);

        Assert.Equal(rawKey, unprotectedValue);
    }

    [Fact]
    public void Protect_ReturnsValueDifferentFromInput()
    {
        const string rawKey = "super-secret-api-key";

        var protectedValue = _service.Protect(rawKey);

        Assert.NotEqual(rawKey, protectedValue);
    }

    public void Dispose()
    {
        if (Directory.Exists(_keyRingDirectory))
        {
            Directory.Delete(_keyRingDirectory, recursive: true);
        }
    }
}
