import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { SearchIconsResultDto } from '../../../dtos/searchIconsResultDto';
import { CardModel } from '../../../models/card.Model';
import { IconService } from '../../../services/icon_services/Icon.service';
import { IconModel } from '../../../models/icon.Model';
import { CardRulesService } from '../../../services/card_services/card-rules.service';
import { CardTypes } from '../../../enums/card.types.enum';
import { WeatherProviderService } from '../../../services/weather_services/weather-provider.service';
import { WeatherDataDtoFactory } from '../../../factory/weatherDtoFactory';
import { TextStringsUtil } from '../../../constants/text.strings.util';

@Injectable()
export class EditCardDialogFacade {
  readonly icons$: Observable<SearchIconsResultDto | null>;
  card: CardModel = new CardModel();

  countryInput = '';
  cityInput = '';
  locationErrorMessage = '';

  #iconService = inject(IconService);
  #CardRulesService = inject(CardRulesService);
  #weatherProviderService = inject(WeatherProviderService);

  constructor() {
    this.icons$ = this.#iconService.icons$;
  }

  get canSubmit(): boolean {
    return this.#CardRulesService.hasRequiredFields({
      ...this.card,
      name: this.card.name.trim(),
      url: this.card.url.trim(),
    });
  }

  get isWeatherCard(): boolean {
    return this.card.type === CardTypes.Weather;
  }

  onSearch(input: string): void {
    this.#iconService.search(input);
  }

  setIcon(event: string): void {

    const iconData = new IconModel();
    iconData.materialIcon = event;
    iconData.type = "";
    iconData.name = "";
    iconData.base64Data = "";
    iconData.type = "";

    this.card.iconData = iconData;
    this.card.iconData.materialIcon = event;
  }

  reset(initial?: Partial<CardModel>): void {
    this.card = Object.assign(new CardModel(), initial ?? {});
    this.countryInput = '';
    this.cityInput = '';
    this.locationErrorMessage = '';
  }

  buildSubmitPayload(id: number): CardModel {
    this.card.id = id;
    return this.card;
  }

  async saveLocation(cardId: number): Promise<boolean> {
    const country = this.countryInput?.trim();
    const city = this.cityInput?.trim();

    if (!country && !city) {
      this.locationErrorMessage = '';
      return true;
    }
    if (!country || !city) {
      this.locationErrorMessage = TextStringsUtil.DialogEditCardLocationBothFieldsRequiredMessage;
      return false;
    }

    this.locationErrorMessage = '';
    const location = `${country},${city}`;
    let [weather, status] = await this.#weatherProviderService.getWeather(location);
    let isFromProvider = false;

    if (status !== 200) {
      [weather, status] = await this.#weatherProviderService.getVisualCrossingData(location);
      isFromProvider = true;
    }

    if (status === 200 && weather !== undefined) {
      weather.cardId = cardId;
      if (isFromProvider) {
        const dto = WeatherDataDtoFactory.createDto(weather);
        await this.#weatherProviderService.save(dto);
      }
      this.countryInput = '';
      this.cityInput = '';
      return true;
    }

    if (status === 401 || status === 412) {
      this.locationErrorMessage = TextStringsUtil.DialogWeatherProviderLocationSaveInvalidKeyFailedMessage;
    } else if (status === 404) {
      this.locationErrorMessage = TextStringsUtil.DialogWeatherProviderLocationSaveLocationNotFoundFailedMessage;
    } else {
      this.locationErrorMessage = TextStringsUtil.DialogWeatherProviderLocationSaveFailedMessage;
    }
    return false;
  }
}
