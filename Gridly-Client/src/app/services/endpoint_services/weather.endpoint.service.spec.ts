import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { urlConstants } from '../../constants/url.constants';
import { WeatherDataModel } from '../../models/weatherData.Model';
import { WeatherEndpointService } from './weather.endpoint.service';

describe('WeatherEndpointService', () => {
  let service: WeatherEndpointService;
  let httpMock: HttpTestingController;

  const cardId = 1;

  const weather: WeatherDataModel = {
    id: 1,
    address: 'Sweden,Stockholm',
    timezone: 'Europe/Stockholm',
    description: 'Clear',
    temp: 20,
    feelsLike: 19,
    humidity: 50,
    windSpeed: 5,
    windDir: 180,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(WeatherEndpointService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('sends a GET request to the weather endpoint with the address as a param', () => {
    let result: WeatherDataModel | undefined;

    service.get('Sweden,Stockholm').subscribe((res) => (result = res));

    const req = httpMock.expectOne(
      (r) => r.url === urlConstants.weather.get && r.params.get('Address') === 'Sweden,Stockholm'
    );
    expect(req.request.method).toBe('GET');
    req.flush(weather);

    expect(result).toEqual(weather);
  });

  it('sends a GET request to fetch stored weather data', () => {
    let result: WeatherDataModel[] | undefined;

    service.getStoredWeatherData().subscribe((res) => (result = res));

    const req = httpMock.expectOne(urlConstants.weather.getStoredWeatherData);
    expect(req.request.method).toBe('GET');
    req.flush([weather]);

    expect(result).toEqual([weather]);
  });

  it('sends a GET request to the visual crossing endpoint with the address as a param', () => {
    let result: WeatherDataModel | undefined;

    service.getvisualcrossingdata('Sweden,Stockholm').subscribe((res) => (result = res));

    const req = httpMock.expectOne(
      (r) => r.url === urlConstants.weather.getVisualCrossingData && r.params.get('Address') === 'Sweden,Stockholm'
    );
    expect(req.request.method).toBe('GET');
    req.flush(weather);

    expect(result).toEqual(weather);
  });

  it('sends a POST request to save weather data with the weather wrapped in the body', () => {
    let completed = false;

    service.save(weather,cardId).subscribe(() => (completed = true));

    const req = httpMock.expectOne(urlConstants.weather.save);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ weather, cardId });
    req.flush(null);

    expect(completed).toBe(true);
  });
});
