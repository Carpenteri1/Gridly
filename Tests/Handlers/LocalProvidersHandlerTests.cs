using Gridly.Dtos;
using Gridly.Enums;
using Gridly.Handlers;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Handlers;

public class LocalProvidersHandlerTests
{
    [Fact]
    public async Task Handle_WhenProviderIsBlank_ReturnsBadRequest()
    {
        var repository = new FakeLocalProvidersRepository();
        var handler = new LocalProvidersHandler(repository);

        var result = await handler.Handle(new GetLocalProviderKeyStatusQuery { Provider = "  " }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Handle_WhenNoKeyIsStored_ReturnsOkWithUnknownStatus()
    {
        var repository = new FakeLocalProvidersRepository();
        var handler = new LocalProvidersHandler(repository);

        var result = await handler.Handle(new GetLocalProviderKeyStatusQuery { Provider = "VisualCrossing" }, CancellationToken.None);

        var status = ResultAssertions.AssertOk<ProviderKeyStatusModel>(result);
        Assert.False(status.Exists);
        Assert.Equal(ProvidersKeyStatusEnum.Unknown, status.KeyStatus);
        Assert.Equal(0, repository.UpdateStatusCallCount);
    }

    [Fact]
    public async Task Handle_WhenStoredKeyIsValid_ReturnsOkWithValidStatusAndUpdatesStatus()
    {
        var repository = new FakeLocalProvidersRepository
        {
            StoredKey = new ProviderKeyDtoModel
            {
                Provider = "VisualCrossing",
                EncryptedKey = "encrypted",
                Status = nameof(ProvidersKeyStatusEnum.Valid),
            },
        };
        var handler = new LocalProvidersHandler(repository);

        var result = await handler.Handle(new GetLocalProviderKeyStatusQuery { Provider = "VisualCrossing" }, CancellationToken.None);

        var status = ResultAssertions.AssertOk<ProviderKeyStatusModel>(result);
        Assert.True(status.Exists);
        Assert.Equal(ProvidersKeyStatusEnum.Valid, status.KeyStatus);
        Assert.Equal(1, repository.UpdateStatusCallCount);
        Assert.Equal(nameof(ProvidersKeyStatusEnum.Valid), repository.LastUpdatedStatus);
    }

    [Fact]
    public async Task Handle_WhenStoredKeyIsNotValid_ReturnsOkWithInvalidStatus()
    {
        var repository = new FakeLocalProvidersRepository
        {
            StoredKey = new ProviderKeyDtoModel
            {
                Provider = "VisualCrossing",
                EncryptedKey = "encrypted",
                Status = nameof(ProvidersKeyStatusEnum.Unknown),
            },
        };
        var handler = new LocalProvidersHandler(repository);

        var result = await handler.Handle(new GetLocalProviderKeyStatusQuery { Provider = "VisualCrossing" }, CancellationToken.None);

        var status = ResultAssertions.AssertOk<ProviderKeyStatusModel>(result);
        Assert.True(status.Exists);
        Assert.Equal(ProvidersKeyStatusEnum.Invalid, status.KeyStatus);
        Assert.Equal(nameof(ProvidersKeyStatusEnum.Invalid), repository.LastUpdatedStatus);
    }
}
