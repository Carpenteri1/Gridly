using Gridly.Commands;
using Gridly.Constants;
using Gridly.Dtos;
using Gridly.Enums;
using Gridly.Handlers;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Handlers;

public class WeatherHandlerTests
{
    private static WeatherModel MakeWeather(string location = "Stockholm") =>
        new()
        {
            CardId = 1,
            Location = location,
            Address = location,
            Timezone = "Europe/Stockholm",
            Description = "clear",
            CurrentConditions = new CurrentConditionsModel
            {
                Conditions = "clear",
                Temp = 20,
                FeelsLike = 20,
                Humidity = 50,
                WindSpeed = 5,
                WindDir = 180
            }
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
        FakeLocalProvidersRepository? providersRepository = null) =>
        new(
            endPoint ?? new FakeWeatherEndPoint(),
            weatherRepository ?? new FakeWeatherRepository(),
            providersRepository ?? new FakeLocalProvidersRepository(),
            new FakeProviderKeysProtectionService());

    [Fact]
    public async Task HandleGetWeather_WhenCacheIsFresh_ReturnsCachedWeather()
    {
        var repository = new FakeWeatherRepository();
        var weather = MakeWeather();
        repository.Seed("Stockholm", weather, DateTime.UtcNow.AddMinutes(-5));
        repository.CardIdSeed(1, weather, DateTime.UtcNow.AddMinutes(-5));
        var handler = MakeHandler(weatherRepository: repository);

        var result = await handler.Handle(new GetWeatherQuery { SearchTerm = "Stockholm" }, CancellationToken.None);
        var payload = ResultAssertions.AssertOk<WeatherModel>(result);

        Assert.Equal("Stockholm", payload.Location);
    }

    [Fact]
    public async Task HandleGetWeather_WhenCacheIsStale_ReturnsNotFound()
    {
        var repository = new FakeWeatherRepository();
        repository.Seed("Stockholm", MakeWeather(), DateTime.UtcNow.AddHours(-9));
        repository.CardIdSeed(1, MakeWeather(), DateTime.UtcNow.AddHours(-9));
        var handler = MakeHandler(weatherRepository: repository);

        var result = await handler.Handle(new GetWeatherQuery { SearchTerm = "Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task HandleGetWeather_WhenNothingStored_ReturnsNotFound()
    {
        var handler = MakeHandler();

        var result = await handler.Handle(new GetWeatherQuery { SearchTerm = "Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task HandleGetVisualCrossingData_WhenSuccessful_ReturnsWeatherAndMarksKeyValid()
    {
        var weather = MakeWeather();
        var endPoint = new FakeWeatherEndPoint { Result = (StatusCodes.Status200OK, weather) };
        var repository = new FakeWeatherRepository();
        var providersRepository = MakeProvidersRepository();
        var handler = MakeHandler(endPoint, repository, providersRepository);

        var result = await handler.Handle(new GetVisualCrossingDataQuery { SearchTerm = "Stockholm" }, CancellationToken.None);
        var payload = ResultAssertions.AssertOk<WeatherModel>(result);
        
        Assert.Equal("Stockholm", payload.Location);
        Assert.Equal(1, endPoint.GetCallCount);
        Assert.Equal(1, providersRepository.UpdateStatusCallCount);
        Assert.Equal(nameof(ProvidersKeyStatusEnum.Valid), providersRepository.LastUpdatedStatus);
    }

    [Fact]
    public async Task HandleGetVisualCrossingData_WhenKeyIsInvalid_MarksKeyInvalidAndReturns401()
    {
        var endPoint = new FakeWeatherEndPoint { Result = (StatusCodes.Status401Unauthorized, null) };
        var apiKeyRepository = MakeProvidersRepository();
        var handler = MakeHandler(endPoint, providersRepository: apiKeyRepository);

        var result = await handler.Handle(new GetVisualCrossingDataQuery { SearchTerm = "Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status401Unauthorized);
        Assert.Equal(1, apiKeyRepository.UpdateStatusCallCount);
        Assert.Equal(nameof(ProvidersKeyStatusEnum.Invalid), apiKeyRepository.LastUpdatedStatus);
    }

    [Fact]
    public async Task HandleGetVisualCrossingData_WhenNoKeyConfigured_Returns401WithoutTouchingKeyStatus()
    {
        var endPoint = new FakeWeatherEndPoint { Result = (StatusCodes.Status401Unauthorized, null) };
        var apiKeyRepository = new FakeLocalProvidersRepository();
        var handler = MakeHandler(endPoint, providersRepository: apiKeyRepository);

        var result = await handler.Handle(new GetVisualCrossingDataQuery { SearchTerm = "Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status401Unauthorized);
        Assert.Equal(0, apiKeyRepository.UpdateStatusCallCount);
    }

    [Fact]
    public async Task HandleGetVisualCrossingData_WhenProviderDownAndStaleDataExists_ReturnsBadRequest()
    {
        var endPoint = new FakeWeatherEndPoint { Result = (StatusCodes.Status500InternalServerError, null) };
        var repository = new FakeWeatherRepository();
        var staleWeather = MakeWeather();
        repository.Seed("Stockholm", staleWeather, DateTime.UtcNow.AddHours(-3));
        repository.CardIdSeed(1, staleWeather, DateTime.UtcNow.AddHours(-3));
        var handler = MakeHandler(endPoint, repository, MakeProvidersRepository());

        var result = await handler.Handle(new GetVisualCrossingDataQuery { SearchTerm = "Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status400BadRequest);
        Assert.Equal(0, repository.UpsertCallCount);
    }

    [Fact]
    public async Task HandleGetVisualCrossingData_WhenProviderDownAndNoStaleData_ReturnsBadRequest()
    {
        var endPoint = new FakeWeatherEndPoint { Result = (StatusCodes.Status500InternalServerError, null) };
        var handler = MakeHandler(endPoint, providersRepository: MakeProvidersRepository());

        var result = await handler.Handle(new GetVisualCrossingDataQuery { SearchTerm = "Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status400BadRequest);
    }
}
