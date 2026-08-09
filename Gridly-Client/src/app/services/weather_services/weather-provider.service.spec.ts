import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { WeatherEndpointService } from '../endpoint_services/weather.endpoint.service';
import { WeatherDataModel } from '../../models/weatherData.Model';
import { WeatherProviderService } from './weather-provider.service';

describe('WeatherProviderService', () => {
  let service: WeatherProviderService;

  const weather: WeatherDataModel = {
    id: 1,
    cardId: 1,
    address: 'Sweden,Stockholm',
    timezone: 'Europe/Stockholm',
    description: 'Clear',
    temp: 20,
    feelsLike: 19,
    humidity: 50,
    windSpeed: 5,
    windDir: 180,
  };

  const endpointMock = {
    save: jest.fn(),
    get: jest.fn(),
    getStoredWeatherData: jest.fn(),
    getvisualcrossingdata: jest.fn(),
  };

  beforeEach(() => {
    jest.clearAllMocks();
    endpointMock.getStoredWeatherData.mockReturnValue(of([weather]));

    TestBed.configureTestingModule({
      providers: [
        WeatherProviderService,
        { provide: WeatherEndpointService, useValue: endpointMock },
      ],
    });

    service = TestBed.inject(WeatherProviderService);
  });

  it('loads stored weather data on construction', () => {
    expect(endpointMock.getStoredWeatherData).toHaveBeenCalledTimes(1);
    expect(service.storedWeatherData()).toEqual([weather]);
  });

  it('refreshes the stored weather data stream on demand', () => {
    endpointMock.getStoredWeatherData.mockReturnValue(of([]));

    service.refresh();

    expect(endpointMock.getStoredWeatherData).toHaveBeenCalledTimes(2);
    expect(service.storedWeatherData()).toEqual([]);
  });

  it('saves weather data through the endpoint', async () => {
    endpointMock.save.mockReturnValue(of(undefined));

    await service.save(weather);

    expect(endpointMock.save).toHaveBeenCalledWith(weather);
  });

  it('returns weather data with a 200 status on a successful getWeather call', async () => {
    endpointMock.get.mockReturnValue(of(weather));

    const [result, status] = await service.getWeather('Sweden,Stockholm');

    expect(result).toEqual(weather);
    expect(status).toBe(200);
  });

  it('returns undefined and the http status when getWeather fails with an HttpErrorResponse', async () => {
    endpointMock.get.mockReturnValue(throwError(() => new HttpErrorResponse({ status: 404 })));

    const [result, status] = await service.getWeather('Sweden,Stockholm');

    expect(result).toBeUndefined();
    expect(status).toBe(404);
  });

  it('returns status 0 when getWeather fails with a non-http error', async () => {
    endpointMock.get.mockReturnValue(throwError(() => new Error('boom')));

    const [result, status] = await service.getWeather('Sweden,Stockholm');

    expect(result).toBeUndefined();
    expect(status).toBe(0);
  });

  it('returns weather data with a 200 status on a successful getVisualCrossingData call', async () => {
    endpointMock.getvisualcrossingdata.mockReturnValue(of(weather));

    const [result, status] = await service.getVisualCrossingData('Sweden,Stockholm');

    expect(result).toEqual(weather);
    expect(status).toBe(200);
  });

  it('returns undefined and the http status when getVisualCrossingData fails with an HttpErrorResponse', async () => {
    endpointMock.getvisualcrossingdata.mockReturnValue(throwError(() => new HttpErrorResponse({ status: 401 })));

    const [result, status] = await service.getVisualCrossingData('Sweden,Stockholm');

    expect(result).toBeUndefined();
    expect(status).toBe(401);
  });
});
