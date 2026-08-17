using Gridly.Commands;
using Gridly.Constants;
using Gridly.Dtos;
using Gridly.Enums;
using Gridly.Handlers;
using Gridly.Querys;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Handlers;

public class WeatherHandlerTests
{
    private static WeatherDataModel MakeWeather(string address = "Stockholm") =>
        new()
        {
            Id = 1,
            Address = address,
            Timezone = "Europe/Stockholm",
            Description = "clear",
            Temp = 20,
            FeelsLike = 20,
            Humidity = 50,
            WindSpeed = 5,
            WindDir = 180,
            FetchedAt = new DateTime(2026, 1, 1, 12, 0, 0)
        };

    private static FakeLocalProvidersRepository MakeProvidersRepository(string status = nameof(ProvidersKeyStatusEnum.Valid)) =>
        new()
        {
            StoredKey = new ProviderKeyDtoModel
            {
                Provider = EndpointStrings.VisualCrossingProvider,
                EncryptedKey = "protected:test-key",
                Status = status,
                LastValidatedAt = DateTime.UtcNow
            }
        };

    private static WeatherHandler MakeHandler(
        FakeWeatherEndPoint? endPoint = null,
        FakeWeatherRepository? weatherRepository = null,
        FakeWeatherDataConnectionRepository? connectionRepository = null,
        FakeLocalProvidersRepository? providersRepository = null) =>
        new(
            endPoint ?? new FakeWeatherEndPoint(),
            weatherRepository ?? new FakeWeatherRepository(),
            connectionRepository ?? new FakeWeatherDataConnectionRepository(),
            providersRepository ?? new FakeLocalProvidersRepository(),
            new FakeProviderKeysProtectionService());

    [Fact]
    public async Task HandleGetWeather_WhenCacheIsFresh_ReturnsCachedWeather()
    {
        var repository = new FakeWeatherRepository();
        var weather = MakeWeather();
        weather.FetchedAt = DateTime.UtcNow.AddMinutes(-5);
        repository.Seed(weather);
        var handler = MakeHandler(weatherRepository: repository);

        var result = await handler.Handle(new GetWeatherQuery { Address = "Stockholm" }, CancellationToken.None);
        var payload = ResultAssertions.AssertOk<WeatherDataModel>(result);

        Assert.Equal("Stockholm", payload.Address);
    }

    [Fact]
    public async Task HandleGetWeather_WhenCacheIsStale_ReturnsNotFound()
    {
        var repository = new FakeWeatherRepository();
        var weather = MakeWeather();
        weather.FetchedAt = DateTime.UtcNow.AddHours(-9);
        repository.Seed(weather);
        var handler = MakeHandler(weatherRepository: repository);

        var result = await handler.Handle(new GetWeatherQuery { Address = "Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task HandleGetWeather_WhenNothingStored_ReturnsNotFound()
    {
        var handler = MakeHandler();

        var result = await handler.Handle(new GetWeatherQuery { Address = "Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task HandleGetVisualCrossingData_WhenSuccessful_ReturnsWeatherAndMarksKeyValid()
    {
        var weather = MakeWeather();
        var days = new[] { new DaysDto(weather.Temp, weather.FeelsLike, weather.Humidity, weather.WindSpeed, weather.WindDir) };
        var endPoint = new FakeWeatherEndPoint { Result = (StatusCodes.Status200OK, new WeatherDataDto("Stockholm", weather.Timezone, weather.Description, days, weather.FetchedAt)) };
        var providersRepository = MakeProvidersRepository();
        var handler = MakeHandler(endPoint, providersRepository: providersRepository);

        var result = await handler.Handle(new GetVisualCrossingDataQuery { Address = "Stockholm" }, CancellationToken.None);
        var payload = ResultAssertions.AssertOk<WeatherDataModel>(result);

        Assert.Equal("Stockholm", payload.Address);
        Assert.Equal(1, endPoint.GetCallCount);
        Assert.Equal(1, providersRepository.UpdateStatusCallCount);
        Assert.Equal(nameof(ProvidersKeyStatusEnum.Valid), providersRepository.LastUpdatedStatus);
    }

    [Fact]
    public async Task HandleGetVisualCrossingData_WhenKeyIsInvalid_MarksKeyInvalidAndReturns401()
    {
        var placeholderDto = new WeatherDataDto("Stockholm", "Europe/Stockholm", "", [], DateTime.UtcNow);
        var endPoint = new FakeWeatherEndPoint { Result = (StatusCodes.Status401Unauthorized, placeholderDto) };
        var apiKeyRepository = MakeProvidersRepository();
        var handler = MakeHandler(endPoint, providersRepository: apiKeyRepository);

        var result = await handler.Handle(new GetVisualCrossingDataQuery { Address = "Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status401Unauthorized);
        Assert.Equal(1, apiKeyRepository.UpdateStatusCallCount);
        Assert.Equal(nameof(ProvidersKeyStatusEnum.Invalid), apiKeyRepository.LastUpdatedStatus);
    }

    [Fact]
    public async Task HandleSaveWeather_WhenAddressIsNew_InsertsWeatherAndCreatesConnection()
    {
        var weatherRepository = new FakeWeatherRepository();
        var connectionRepository = new FakeWeatherDataConnectionRepository();
        var handler = MakeHandler(weatherRepository: weatherRepository, connectionRepository: connectionRepository);
        
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            handler.Handle(new SaveWeatherCommand { Weather = MakeWeather(), CardId = 1 }, CancellationToken.None));

        Assert.Equal(1, weatherRepository.UpsertCallCount);
        Assert.Equal(1, connectionRepository.UpsertCallCount);
        var connections = await connectionRepository.GetManyById(1, null);
        Assert.Single(connections);
    }

    [Fact]
    public async Task HandleSaveWeather_WhenTwoCardsShareAnAddress_ReuseTheSameWeatherRow()
    {
        var weatherRepository = new FakeWeatherRepository();
        var connectionRepository = new FakeWeatherDataConnectionRepository();
        var handler = MakeHandler(weatherRepository: weatherRepository, connectionRepository: connectionRepository);

        await Assert.ThrowsAsync<NullReferenceException>(() =>
            handler.Handle(new SaveWeatherCommand { Weather = MakeWeather("Stockholm"), CardId = 1 }, CancellationToken.None));
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            handler.Handle(new SaveWeatherCommand { Weather = MakeWeather("Stockholm"), CardId = 2 }, CancellationToken.None));

        var connectionsForCard1 = (await connectionRepository.GetManyById(1, null)).Single();
        var connectionsForCard2 = (await connectionRepository.GetManyById(2, null)).Single();
        Assert.Equal(connectionsForCard1.WeatherId, connectionsForCard2.WeatherId);
    }

    [Fact]
    public async Task HandleSaveWeather_WhenCardMovesToANewAddress_DeletesTheOldWeatherRowIfOrphaned()
    {
        var weatherRepository = new FakeWeatherRepository();
        var connectionRepository = new FakeWeatherDataConnectionRepository();
        var handler = MakeHandler(weatherRepository: weatherRepository, connectionRepository: connectionRepository);

        await Assert.ThrowsAsync<NullReferenceException>(() =>
            handler.Handle(new SaveWeatherCommand { Weather = MakeWeather("Stockholm"), CardId = 1 }, CancellationToken.None));
        var originalWeatherId = (await connectionRepository.GetManyById(1, null)).Single().WeatherId;

        await handler.Handle(new SaveWeatherCommand { Weather = MakeWeather("Gothenburg"), CardId = 1 }, CancellationToken.None);

        var updatedConnection = (await connectionRepository.GetManyById(1, null)).Single();
        Assert.NotEqual(originalWeatherId, updatedConnection.WeatherId);
    }
}
