using Gridly.Commands;
using Gridly.Dtos;
using Gridly.Enums;
using Gridly.Handlers;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Handlers;

public class ProviderKeysHandlerTests
{
    private const string Provider = "VisualCrossing";

    private static ProvidersHandler CreateHandler(
        FakeProvidersRepository repository,
        FakeProvidersEndPoint endPoint) =>
        new(repository, new FakeProviderKeysProtectionService(), endPoint);

    private static ProviderKeyDtoModel StoredKey(ProvidersKeyStatusEnum keyStatus, DateTime? lastValidatedAt) =>
        new()
        {
            Provider = Provider,
            EncryptedKey = "protected:secret-key",
            Status = keyStatus.ToString(),
            LastValidatedAt = lastValidatedAt
        };

    [Fact]
    public async Task HandleSaveProviderKey_WhenKeyProvided_EncryptsAndStoresWithUnknownStatus()
    {
        var repository = new FakeProvidersRepository();
        var handler = CreateHandler(repository, new FakeProvidersEndPoint());

        var result = await handler.Handle(
            new SaveProviderKeyCommand { Provider = Provider, RawKey = "secret-key" },
            CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status200OK);
        Assert.Equal(1, repository.UpsertCallCount);
        Assert.Equal("protected:secret-key", repository.StoredKey!.EncryptedKey);
        Assert.Equal(nameof(ProvidersKeyStatusEnum.Unknown), repository.StoredKey!.Status);
    }

    [Fact]
    public async Task HandleSaveProviderKey_TrimsSurroundingWhitespaceBeforeEncrypting()
    {
        var repository = new FakeProvidersRepository();
        var handler = CreateHandler(repository, new FakeProvidersEndPoint());

        var result = await handler.Handle(
            new SaveProviderKeyCommand { Provider = Provider, RawKey = "  secret-key\n" },
            CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status200OK);
        Assert.Equal("protected:secret-key", repository.StoredKey!.EncryptedKey);
    }

    [Fact]
    public async Task HandleSaveProviderKey_WhenKeyIsBlank_ReturnsBadRequestWithoutStoring()
    {
        var repository = new FakeProvidersRepository();
        var handler = CreateHandler(repository, new FakeProvidersEndPoint());

        var result = await handler.Handle(
            new SaveProviderKeyCommand { Provider = Provider, RawKey = "   " },
            CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status400BadRequest);
        Assert.Equal(0, repository.UpsertCallCount);
    }

    [Fact]
    public async Task HandleGetStatus_WhenNoKeyStored_ReturnsExistsFalseWithoutCallingProvider()
    {
        var endPoint = new FakeProvidersEndPoint();
        var handler = CreateHandler(new FakeProvidersRepository(), endPoint);

        var result = await handler.Handle(new GetProviderKeyStatusQuery { Provider = Provider }, CancellationToken.None);
        var payload = ResultAssertions.AssertOk<ProviderKeyStatusModel>(result);

        Assert.False(payload.Exists);
        Assert.Equal(ProvidersKeyStatusEnum.Unknown, payload.KeyStatus);
        Assert.Equal(0, endPoint.ValidateCallCount);
    }

    [Fact]
    public async Task HandleGetStatus_WhenStatusUnknown_ValidatesAndPersistsValid()
    {
        var repository = new FakeProvidersRepository
        {
            StoredKey = StoredKey(ProvidersKeyStatusEnum.Unknown, lastValidatedAt: null)
        };
        var endPoint = new FakeProvidersEndPoint { Result = StatusCodes.Status200OK };
        var handler = CreateHandler(repository, endPoint);

        var result = await handler.Handle(new GetProviderKeyStatusQuery { Provider = Provider }, CancellationToken.None);
        var payload = ResultAssertions.AssertOk<ProviderKeyStatusModel>(result);

        Assert.True(payload.Exists);
        Assert.Equal(ProvidersKeyStatusEnum.Valid, payload.KeyStatus);
        Assert.Equal(1, endPoint.ValidateCallCount);
        Assert.Equal(1, repository.UpdateStatusCallCount);
        Assert.Equal(nameof(ProvidersKeyStatusEnum.Valid), repository.LastUpdatedStatus);
    }

    [Fact]
    public async Task HandleGetStatus_WhenProviderRejectsKey_ReturnsUnauthorizedWithoutPersisting()
    {
        var repository = new FakeProvidersRepository
        {
            StoredKey = StoredKey(ProvidersKeyStatusEnum.Unknown, lastValidatedAt: null)
        };
        var endPoint = new FakeProvidersEndPoint { Result = StatusCodes.Status401Unauthorized };
        var handler = CreateHandler(repository, endPoint);

        var result = await handler.Handle(new GetProviderKeyStatusQuery { Provider = Provider }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status401Unauthorized);
        Assert.Equal(0, repository.UpdateStatusCallCount);
    }

    [Fact]
    public async Task HandleGetStatus_WhenKeyIsStored_ValidatesProvider()
    {
        var repository = new FakeProvidersRepository
        {
            StoredKey = StoredKey(ProvidersKeyStatusEnum.Valid, DateTime.UtcNow.AddMinutes(-5))
        };
        var endPoint = new FakeProvidersEndPoint();
        var handler = CreateHandler(repository, endPoint);

        var result = await handler.Handle(new GetProviderKeyStatusQuery { Provider = Provider }, CancellationToken.None);
        var payload = ResultAssertions.AssertOk<ProviderKeyStatusModel>(result);

        Assert.True(payload.Exists);
        Assert.Equal(ProvidersKeyStatusEnum.Valid, payload.KeyStatus);
        Assert.Equal(1, endPoint.ValidateCallCount);
        Assert.Equal(1, repository.UpdateStatusCallCount);
    }

    [Fact]
    public async Task HandleGetStatus_WhenValidationReturnsUnauthorized_ReturnsUnauthorized()
    {
        var repository = new FakeProvidersRepository
        {
            StoredKey = StoredKey(ProvidersKeyStatusEnum.Valid, DateTime.UtcNow.AddHours(-2))
        };
        var endPoint = new FakeProvidersEndPoint { Result = StatusCodes.Status401Unauthorized };
        var handler = CreateHandler(repository, endPoint);

        var result = await handler.Handle(new GetProviderKeyStatusQuery { Provider = Provider }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status401Unauthorized);
        Assert.Equal(1, endPoint.ValidateCallCount);
        Assert.Equal(0, repository.UpdateStatusCallCount);
    }

    [Fact]
    public async Task HandleGetStatus_WhenProviderUnavailable_ReturnsBadRequestWithoutPersisting()
    {
        var repository = new FakeProvidersRepository
        {
            StoredKey = StoredKey(ProvidersKeyStatusEnum.Valid, DateTime.UtcNow.AddHours(-2))
        };
        var endPoint = new FakeProvidersEndPoint { Result = StatusCodes.Status500InternalServerError };
        var handler = CreateHandler(repository, endPoint);

        var result = await handler.Handle(new GetProviderKeyStatusQuery { Provider = Provider }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status400BadRequest);
        Assert.Equal(0, repository.UpdateStatusCallCount);
    }

    [Fact]
    public async Task HandleGetStatus_WhenProviderForbidsKey_ReturnsForbiddenWithoutPersisting()
    {
        var repository = new FakeProvidersRepository
        {
            StoredKey = StoredKey(ProvidersKeyStatusEnum.Unknown, lastValidatedAt: null)
        };
        var endPoint = new FakeProvidersEndPoint { Result = StatusCodes.Status403Forbidden };
        var handler = CreateHandler(repository, endPoint);

        var result = await handler.Handle(new GetProviderKeyStatusQuery { Provider = Provider }, CancellationToken.None);

        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
        Assert.Equal(0, repository.UpdateStatusCallCount);
    }
}
