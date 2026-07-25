using Gridly.Commands;
using Gridly.Handlers;
using Gridly.Models;
using Gridly.Querys;
using Gridly.Tests.Infrastructure;

namespace Gridly.Tests.Handlers;

public class WeatherHandlerTests
{
    private static WeatherModel MakeWeather(string location = "Stockholm") =>
        new() { Location = location, Address = location, Timezone = "Europe/Stockholm", Description = "clear" };

    [Fact]
    public async Task HandleGetWeather_WhenCacheIsFresh_ReturnsCachedWeather()
    {
        var repository = new FakeWeatherRepository();
        var weather = MakeWeather();
        repository.Seed("Stockholm", weather, DateTime.UtcNow.AddMinutes(-5));
        var handler = new WeatherHandler(new FakeWeatherEndPoint(), repository, new FakeApiKeyRepository());

        var result = await handler.Handle(new GetWeatherQuery { SearchTerm = "Stockholm" }, CancellationToken.None);
        var payload = ResultAssertions.AssertOk<WeatherModel>(result);

        Assert.Equal("Stockholm", payload.Location);
    }

    [Fact]
    public async Task HandleGetWeather_WhenCacheIsStale_ReturnsNotFound()
    {
        var repository = new FakeWeatherRepository();
        repository.Seed("Stockholm", MakeWeather(), DateTime.UtcNow.AddMinutes(-45));
        var handler = new WeatherHandler(new FakeWeatherEndPoint(), repository, new FakeApiKeyRepository());

        var result = await handler.Handle(new GetWeatherQuery { SearchTerm = "Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task HandleGetWeather_WhenNothingStored_ReturnsNotFound()
    {
        var handler = new WeatherHandler(new FakeWeatherEndPoint(), new FakeWeatherRepository(), new FakeApiKeyRepository());

        var result = await handler.Handle(new GetWeatherQuery { SearchTerm = "Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status404NotFound);
    }

    [Fact]
    public async Task HandleGetVisualCrossingData_WhenSuccessful_StoresAndReturnsWeather()
    {
        var weather = MakeWeather();
        var endPoint = new FakeWeatherEndPoint { Result = (WeatherFetchStatus.Success, weather) };
        var repository = new FakeWeatherRepository();
        var handler = new WeatherHandler(endPoint, repository, new FakeApiKeyRepository());

        var result = await handler.Handle(new GetVisualCrossingDataQuery { SearchTerm = "Stockholm" }, CancellationToken.None);
        var payload = ResultAssertions.AssertOk<WeatherModel>(result);

        Assert.Equal("Stockholm", payload.Location);
        Assert.Equal(1, repository.UpsertCallCount);
        Assert.Equal("Stockholm", repository.LastUpsertedLocation);
    }

    [Fact]
    public async Task HandleGetVisualCrossingData_WhenKeyIsInvalid_MarksKeyInvalidAndReturns401()
    {
        var endPoint = new FakeWeatherEndPoint { Result = (WeatherFetchStatus.InvalidApiKey, null) };
        var apiKeyRepository = new FakeApiKeyRepository();
        var handler = new WeatherHandler(endPoint, new FakeWeatherRepository(), apiKeyRepository);

        var result = await handler.Handle(new GetVisualCrossingDataQuery { SearchTerm = "Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status401Unauthorized);
        Assert.Equal(1, apiKeyRepository.UpdateStatusCallCount);
        Assert.Equal(ApiKeyStatus.Invalid, apiKeyRepository.LastUpdatedStatus);
    }

    [Fact]
    public async Task HandleGetVisualCrossingData_WhenNoKeyConfigured_Returns412WithoutTouchingKeyStatus()
    {
        var endPoint = new FakeWeatherEndPoint { Result = (WeatherFetchStatus.NoApiKey, null) };
        var apiKeyRepository = new FakeApiKeyRepository();
        var handler = new WeatherHandler(endPoint, new FakeWeatherRepository(), apiKeyRepository);

        var result = await handler.Handle(new GetVisualCrossingDataQuery { SearchTerm = "Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status412PreconditionFailed);
        Assert.Equal(0, apiKeyRepository.UpdateStatusCallCount);
    }

    [Fact]
    public async Task HandleGetVisualCrossingData_WhenProviderDownAndStaleDataExists_FallsBackToStaleWeather()
    {
        var endPoint = new FakeWeatherEndPoint { Result = (WeatherFetchStatus.ProviderUnavailable, null) };
        var repository = new FakeWeatherRepository();
        var staleWeather = MakeWeather();
        repository.Seed("Stockholm", staleWeather, DateTime.UtcNow.AddHours(-3));
        var handler = new WeatherHandler(endPoint, repository, new FakeApiKeyRepository());

        var result = await handler.Handle(new GetVisualCrossingDataQuery { SearchTerm = "Stockholm" }, CancellationToken.None);
        var payload = ResultAssertions.AssertOk<WeatherModel>(result);

        Assert.Equal("Stockholm", payload.Location);
        Assert.Equal(0, repository.UpsertCallCount);
    }

    [Fact]
    public async Task HandleGetVisualCrossingData_WhenProviderDownAndNoStaleData_Returns503()
    {
        var endPoint = new FakeWeatherEndPoint { Result = (WeatherFetchStatus.ProviderUnavailable, null) };
        var handler = new WeatherHandler(endPoint, new FakeWeatherRepository(), new FakeApiKeyRepository());

        var result = await handler.Handle(new GetVisualCrossingDataQuery { SearchTerm = "Stockholm" }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status503ServiceUnavailable);
    }

    [Fact]
    public async Task HandleSaveWeather_UpsertsWeatherForLocation()
    {
        var repository = new FakeWeatherRepository();
        var handler = new WeatherHandler(new FakeWeatherEndPoint(), repository, new FakeApiKeyRepository());
        var weather = MakeWeather();

        var result = await handler.Handle(new SaveWeatherCommand { Location = "Stockholm", Weather = weather }, CancellationToken.None);

        ResultAssertions.AssertStatusCode(result, StatusCodes.Status200OK);
        Assert.Equal(1, repository.UpsertCallCount);
        Assert.Equal("Stockholm", repository.LastUpsertedLocation);
    }
}
