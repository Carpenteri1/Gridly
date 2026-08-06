import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { SearchIconsResultDto } from '../../../dtos/searchIconsResultDto';
import { CardModel } from '../../../models/card.Model';
import { IconService } from '../../../services/icon_services/Icon.service';
import { IconModel } from '../../../models/icon.Model';
import { CardRulesService } from '../../../services/card_services/card-rules.service';
import { CardTypes } from '../../../enums/card.types.enum';
import { WeatherProviderService } from '../../../services/weather_services/weather-provider.service';
import {ProviderKeysService} from "../../../services/provider_key_services/provider-keys.service";

@Injectable()
export class EditCardDialogFacade {
  readonly icons$: Observable<SearchIconsResultDto | null>;
  card: CardModel = new CardModel();

  countryInput = '';
  cityInput = '';
  errorStatus = 0;

  #iconService = inject(IconService);
  #CardRulesService = inject(CardRulesService);
  #weatherProviderService = inject(WeatherProviderService);
  #providerKeyService = inject(ProviderKeysService);

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
    this.errorStatus = 0;
  }

  buildSubmitPayload(id: number): CardModel {
    this.card.id = id;
    return this.card;
  }

  async saveLocation(cardId: number): Promise<boolean> {
    const country = this.countryInput?.trim();
    const city = this.cityInput?.trim();

    if (!country && !city) {
      this.errorStatus = 0;
      return true;
    }
    if (!country || !city) {
      return false;
    }

    const address = `${country},${city}`;
    this.countryInput = '';
    this.cityInput = '';

    let [weather, status] = await this.#weatherProviderService.getWeather(address);

    if (status !== 200) {
      [weather, status] = await this.#weatherProviderService.getVisualCrossingData(address);
      if (status === 401 || status === 412) {
        this.errorStatus = status;
        this.#providerKeyService.promptForInvalidKey();
      }
    }
    if (status === 200 && weather !== undefined) {
      if (weather.cardId !== cardId) {
        weather.cardId = cardId;
        await this.#weatherProviderService.save(weather);
        return true;
      }
    }
    this.errorStatus = status;
    return false;
  }
}
