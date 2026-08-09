import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AsyncPipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { StubTranslatePipe } from '../../../testing/stub-translate.pipe';
import { WeatherProviderService } from '../../../services/weather_services/weather-provider.service';
import { ProviderKeysService } from '../../../services/provider_key_services/provider-keys.service';
import { WeatherDataModel } from '../../../models/weatherData.Model';
import { SetLocationForProviderDialogComponent } from './set-location-for-provider-dialog.component';

describe('SetLocationForProviderDialogComponent', () => {
  let fixture: ComponentFixture<SetLocationForProviderDialogComponent>;
  let component: SetLocationForProviderDialogComponent;

  const weather: WeatherDataModel = {
    id: 1,
    cardId: 99,
    address: 'Sweden,Stockholm',
    timezone: 'Europe/Stockholm',
    description: 'Clear',
    temp: 20,
    feelsLike: 19,
    humidity: 50,
    windSpeed: 5,
    windDir: 180,
  };

  const weatherProviderServiceMock = {
    getWeather: jest.fn(),
    getVisualCrossingData: jest.fn(),
    save: jest.fn(),
  };

  const providerKeysServiceMock = {
    promptForInvalidKey: jest.fn(),
  };

  const translateServiceMock = {
    instant: (key: string) => key,
  };

  beforeEach(() => {
    jest.clearAllMocks();

    TestBed.configureTestingModule({
      imports: [SetLocationForProviderDialogComponent],
      providers: [
        { provide: WeatherProviderService, useValue: weatherProviderServiceMock },
        { provide: ProviderKeysService, useValue: providerKeysServiceMock },
        { provide: TranslateService, useValue: translateServiceMock },
      ],
    }).overrideComponent(SetLocationForProviderDialogComponent, {
      remove: { imports: [TranslatePipe] },
      add: { imports: [AsyncPipe, StubTranslatePipe] },
    });

    fixture = TestBed.createComponent(SetLocationForProviderDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    jest.spyOn(component, 'close').mockImplementation(() => undefined);
    component.id = 99;
  });

  it('does nothing when the country or city input is empty', async () => {
    component.countryInput = '  ';
    component.cityInput = 'Stockholm';

    await component.onSubmit();

    expect(weatherProviderServiceMock.getWeather).not.toHaveBeenCalled();
  });

  it('saves the weather, resets inputs, and closes on a successful lookup', async () => {
    weatherProviderServiceMock.getWeather.mockResolvedValue([{ ...weather, cardId: 1 }, 200]);
    component.countryInput = 'Sweden';
    component.cityInput = 'Stockholm';

    await component.onSubmit();

    expect(weatherProviderServiceMock.getWeather).toHaveBeenCalledWith('Sweden,Stockholm');
    expect(weatherProviderServiceMock.save).toHaveBeenCalledWith(expect.objectContaining({ cardId: 99 }));
    expect(component.countryInput).toBe('');
    expect(component.cityInput).toBe('');
    expect(component.close).toHaveBeenCalled();
  });

  it('does not re-save when the weather already belongs to the current card', async () => {
    weatherProviderServiceMock.getWeather.mockResolvedValue([{ ...weather, cardId: 99 }, 200]);
    component.countryInput = 'Sweden';
    component.cityInput = 'Stockholm';

    await component.onSubmit();

    expect(weatherProviderServiceMock.save).not.toHaveBeenCalled();
    expect(component.close).toHaveBeenCalled();
  });

  it('falls back to visual crossing and prompts for an invalid key on a 401', async () => {
    weatherProviderServiceMock.getWeather.mockResolvedValue([undefined, 401]);
    weatherProviderServiceMock.getVisualCrossingData.mockResolvedValue([undefined, 401]);
    component.countryInput = 'Sweden';
    component.cityInput = 'Stockholm';

    await component.onSubmit();

    expect(weatherProviderServiceMock.getVisualCrossingData).toHaveBeenCalledWith('Sweden,Stockholm');
    expect(providerKeysServiceMock.promptForInvalidKey).toHaveBeenCalled();
    expect(component.errorMessage).toBe('weatherProviderLocationDialog.text.saveInvalidKeyFailedMessage');
    expect(component.close).not.toHaveBeenCalled();
  });

  it('prompts for an invalid key on a 412 from the fallback provider', async () => {
    weatherProviderServiceMock.getWeather.mockResolvedValue([undefined, 500]);
    weatherProviderServiceMock.getVisualCrossingData.mockResolvedValue([undefined, 412]);
    component.countryInput = 'Sweden';
    component.cityInput = 'Stockholm';

    await component.onSubmit();

    expect(providerKeysServiceMock.promptForInvalidKey).toHaveBeenCalled();
    expect(component.errorMessage).toBe('weatherProviderLocationDialog.text.saveInvalidKeyFailedMessage');
  });

  it('sets a not-found error message on a 404', async () => {
    weatherProviderServiceMock.getWeather.mockResolvedValue([undefined, 404]);
    weatherProviderServiceMock.getVisualCrossingData.mockResolvedValue([undefined, 404]);
    component.countryInput = 'Sweden';
    component.cityInput = 'Stockholm';

    await component.onSubmit();

    expect(providerKeysServiceMock.promptForInvalidKey).not.toHaveBeenCalled();
    expect(component.errorMessage).toBe('weatherProviderLocationDialog.text.saveLocationNotFoundFailedMessage');
  });

  it('sets a generic error message for any other failure status', async () => {
    weatherProviderServiceMock.getWeather.mockResolvedValue([undefined, 500]);
    weatherProviderServiceMock.getVisualCrossingData.mockResolvedValue([undefined, 500]);
    component.countryInput = 'Sweden';
    component.cityInput = 'Stockholm';

    await component.onSubmit();

    expect(component.errorMessage).toBe('weatherProviderLocationDialog.text.saveFailedMessage');
  });
});
