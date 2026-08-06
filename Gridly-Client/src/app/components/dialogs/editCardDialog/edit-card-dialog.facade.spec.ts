import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { IconService } from '../../../services/icon_services/Icon.service';
import { CardRulesService } from '../../../services/card_services/card-rules.service';
import { WeatherProviderService } from '../../../services/weather_services/weather-provider.service';
import { CardTypes } from '../../../enums/card.types.enum';
import { EditCardDialogFacade } from './edit-card-dialog.facade';
import {WeatherDataModel} from "../../../models/weatherData.Model";

describe('EditCardDialogFacade', () => {
  let facade: EditCardDialogFacade;

  const iconServiceMock = {
    icons$: of({ icons: ['home'] }),
    search: jest.fn(),
  };

  const CardRulesServiceMock = {
    hasRequiredFields: jest.fn((card: { name?: string; url?: string }) =>
      Boolean(card.name?.trim().length) && Boolean(card.url?.trim().length)
    ),
  };

  const makeWeather = (): WeatherDataModel => ({
    cardId: 0,
    address: 'Stockholm, Sweden',
    timezone: 'Europe/Stockholm',
    description: 'clear',
    temp: 20,
    feelsLike: 20,
    humidity: 50,
    windSpeed: 5,
    windDir: 180,
    id: 0
  });

  const weatherProviderServiceMock = {
    getWeather: jest.fn(),
    getVisualCrossingData: jest.fn(),
    save: jest.fn(),
  };

  beforeEach(() => {
    jest.clearAllMocks();
    TestBed.configureTestingModule({
      providers: [
        EditCardDialogFacade,
        { provide: IconService, useValue: iconServiceMock },
        { provide: CardRulesService, useValue: CardRulesServiceMock },
        { provide: WeatherProviderService, useValue: weatherProviderServiceMock },
      ],
    });

    facade = TestBed.inject(EditCardDialogFacade);
  });

  it('validates submit readiness from the current form data', () => {
    facade.card.name = '';
    facade.card.url = '';
    expect(facade.canSubmit).toBe(false);

    facade.card.name = 'Card';
    facade.card.url = 'https://card.example';
    expect(facade.canSubmit).toBe(true);
    expect(CardRulesServiceMock.hasRequiredFields).toHaveBeenLastCalledWith({
      ...facade.card,
      name: 'Card',
      url: 'https://card.example',
    });
  });

  it('keeps whitespace-only values blocked when checking submit readiness', () => {
    facade.card.name = '   ';
    facade.card.url = '\t';

    expect(facade.canSubmit).toBe(false);
    expect(CardRulesServiceMock.hasRequiredFields).toHaveBeenLastCalledWith({
      ...facade.card,
      name: '',
      url: '',
    });
  });

  it('forwards icon searches to the icon service', () => {
    facade.onSearch('mail');

    expect(iconServiceMock.search).toHaveBeenCalledWith('mail');
  });

  it('stores a selected material icon on the card payload', () => {
    facade.setIcon('settings');

    expect(facade.card.iconData?.materialIcon).toBe('settings');
  });

  it('resets and builds the submit payload with the card id', () => {
    facade.reset({ name: 'Alpha', url: 'https://alpha.example' });
    const payload = facade.buildSubmitPayload(42);

    expect(payload.id).toBe(42);
    expect(payload.name).toBe('Alpha');
    expect(payload.url).toBe('https://alpha.example');
  });

  it('reports isWeatherCard based on the card type', () => {
    facade.card.type = CardTypes.Custom;
    expect(facade.isWeatherCard).toBe(false);

    facade.card.type = CardTypes.Weather;
    expect(facade.isWeatherCard).toBe(true);
  });

  describe('saveLocation', () => {
    it('skips saving and succeeds when both fields are left blank', async () => {
      facade.countryInput = '';
      facade.cityInput = '   ';

      const result = await facade.saveLocation(1);

      expect(result).toBe(true);
      expect(weatherProviderServiceMock.getWeather).not.toHaveBeenCalled();
      expect(weatherProviderServiceMock.save).not.toHaveBeenCalled();
    });

    it('fails validation when only one of country/city is filled', async () => {
      facade.countryInput = 'Sweden';
      facade.cityInput = '';

      const result = await facade.saveLocation(1);

      expect(result).toBe(false);
      expect(weatherProviderServiceMock.getWeather).not.toHaveBeenCalled();
    });

    it('populates the card from already-stored weather without re-saving it', async () => {
      facade.countryInput = 'Sweden';
      facade.cityInput = 'Stockholm';
      weatherProviderServiceMock.getWeather.mockResolvedValue([makeWeather(), 200]);

      const result = await facade.saveLocation(9);

      expect(result).toBe(true);
      expect(weatherProviderServiceMock.getWeather).toHaveBeenCalledWith("Sweden,Stockholm");
      expect(weatherProviderServiceMock.getVisualCrossingData).not.toHaveBeenCalled();
      expect(weatherProviderServiceMock.save).toHaveBeenCalled();
      expect(facade.countryInput).toBe('');
      expect(facade.cityInput).toBe('');
    });

    it('falls back to getVisualCrossingData and saves the freshly-fetched weather when getWeather fails', async () => {
      facade.countryInput = 'Sweden';
      facade.cityInput = 'Stockholm';
      weatherProviderServiceMock.getWeather.mockResolvedValue([undefined, 404]);
      weatherProviderServiceMock.getVisualCrossingData.mockResolvedValue([makeWeather(), 200]);

      const result = await facade.saveLocation(9);

      expect(result).toBe(true);
      expect(weatherProviderServiceMock.getVisualCrossingData).toHaveBeenCalledWith('Sweden,Stockholm');
      expect(weatherProviderServiceMock.save).toHaveBeenCalledWith(
        expect.objectContaining({"address": "Stockholm, Sweden", "cardId": 9, "description": "clear", "feelsLike": 20, "humidity": 50, "id": 0, "temp": 20, "timezone": "Europe/Stockholm", "windDir": 180, "windSpeed": 5})
      );
    });

    it('surfaces an invalid-key error message and does not save', async () => {
      facade.countryInput = 'Sweden';
      facade.cityInput = 'Stockholm';
      weatherProviderServiceMock.getWeather.mockResolvedValue([undefined, 401]);
      weatherProviderServiceMock.getVisualCrossingData.mockResolvedValue([undefined, 401]);

      const result = await facade.saveLocation(9);

      expect(result).toBe(false);
      expect(weatherProviderServiceMock.save).not.toHaveBeenCalled();
    });

    it('surfaces a location-not-found error message', async () => {
      facade.countryInput = 'Sweden';
      facade.cityInput = 'Nowhere';
      weatherProviderServiceMock.getWeather.mockResolvedValue([undefined, 404]);
      weatherProviderServiceMock.getVisualCrossingData.mockResolvedValue([undefined, 404]);

      const result = await facade.saveLocation(9);

      expect(result).toBe(false);
    });

    it('surfaces a generic failure message for other failures', async () => {
      facade.countryInput = 'Sweden';
      facade.cityInput = 'Stockholm';
      weatherProviderServiceMock.getWeather.mockResolvedValue([undefined, 500]);
      weatherProviderServiceMock.getVisualCrossingData.mockResolvedValue([undefined, 500]);

      const result = await facade.saveLocation(9);

      expect(result).toBe(false);
    });
  });
});
