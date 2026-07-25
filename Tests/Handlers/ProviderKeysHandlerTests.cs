using Gridly.Commands;
using Gridly.Handlers;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Tests.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Gridly.Tests.Handlers;

public class ProviderKeysHandlerTests
{
    [Fact]
    public async Task HandleSaveApiKey_WhenKeyProvided_EncryptsAndStoresWithUnknownStatus()
    {
        var repository = new FakeProviderKeysRepository();
        var protection = new FakeProviderKeysProtectionService();
        var handler = new ProviderKeyHandler(repository, protection);

        var result = await handler.Handle(
            new SaveProviderKeyCommand { Provider = "VisualCrossing", RawKey = "secret-key" },
            CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status200OK);
        Assert.Equal(1, repository.UpsertCallCount);
        Assert.NotNull(repository.StoredKey);
        Assert.Equal("protected:secret-key", repository.StoredKey!.EncryptedKey);
        Assert.Equal(ProviderKeyStatusModel.Unknown, repository.StoredKey!.Status);
    }

    [Fact]
    public async Task HandleSaveApiKey_WhenKeyIsBlank_ReturnsBadRequestWithoutStoring()
    {
        var repository = new FakeProviderKeysRepository();
        var handler = new ProviderKeyHandler(repository, new FakeProviderKeysProtectionService());

        var result = await handler.Handle(
            new SaveProviderKeyCommand { Provider = "VisualCrossing", RawKey = "   " },
            CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status400BadRequest);
        Assert.Equal(0, repository.UpsertCallCount);
    }

    [Fact]
    public async Task HandleGetApiKeyStatus_WhenNoKeyStored_ReturnsExistsFalse()
    {
        var handler = new ProviderKeyHandler(new FakeProviderKeysRepository(), new FakeProviderKeysProtectionService());

        var result = await handler.Handle(new GetProviderKeyStatusQuery { Provider = "VisualCrossing" }, CancellationToken.None);
        var payload = GetOkValue(result);

        Assert.False((bool)payload.GetType().GetProperty("exists")!.GetValue(payload)!);
    }

    [Fact]
    public async Task HandleGetApiKeyStatus_WhenKeyStored_ReturnsExistsTrueWithStatus()
    {
        var repository = new FakeProviderKeysRepository();
        await repository.Upsert("VisualCrossing", "protected:secret-key", ProviderKeyStatusModel.Valid);
        var handler = new ProviderKeyHandler(repository, new FakeProviderKeysProtectionService());

        var result = await handler.Handle(new GetProviderKeyStatusQuery { Provider = "VisualCrossing" }, CancellationToken.None);
        var payload = GetOkValue(result);

        Assert.True((bool)payload.GetType().GetProperty("exists")!.GetValue(payload)!);
        Assert.Equal(ProviderKeyStatusModel.Valid, payload.GetType().GetProperty("status")!.GetValue(payload));
    }

    private static object GetOkValue(IResult result)
    {
        var statusResult = Assert.IsAssignableFrom<IStatusCodeHttpResult>(result);
        Assert.Equal(StatusCodes.Status200OK, statusResult.StatusCode);
        var valueResult = Assert.IsAssignableFrom<IValueHttpResult>(result);
        return valueResult.Value!;
    }
}
