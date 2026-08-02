import {Component, EventEmitter, inject, Input, Output} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BaseDialogComponent } from '../../../directives/base-dialog.directive';
import { DialogDirective } from '../../../directives/dialog.directive';
import {WeatherProviderService} from "../../../services/weather_services/weather-provider.service";

@Component({
  selector: 'app-set-location-for-provider-dialog',
  standalone: true,
  imports: [FormsModule, DialogDirective],
  templateUrl: './set-location-for-provider-dialog.component.html',
  styleUrls: ['../../../css/shared.dialog.css'],
})
export class SetLocationForProviderDialogComponent extends BaseDialogComponent{
  @Input() open = false;
  @Input() id = 0;
  @Output() openChange = new EventEmitter<number>();

  #weatherProviderService = inject(WeatherProviderService);

  countryInput!:string;
  cityInput!:string;
  saving = false;
  errorMessage!:string;

  async onSubmit() {
    const country = this.countryInput?.trim();
    const city = this.cityInput?.trim();
    if (!country || !city) return;

    this.saving = true;
    this.errorMessage = '';

    const location = `${country},${city}`;
    let [weather, status] = await this.#weatherProviderService.getWeather(location);

    if (status !== 200) {
      [weather, status] = await this.#weatherProviderService.getVisualCrossingData(location);
    }

    this.saving = false;

    if (status === 200 && weather !== undefined) {
      this.countryInput = '';
      this.cityInput = '';
      weather.cardId = this.id;
      this.#weatherProviderService.save(weather);
      this.close();
    } else if (status === 401 || status === 412) {
      this.errorMessage = this.TextStringsUtil.DialogWeatherProviderLocationSaveInvalidKeyFailedMessage;
    } else if (status === 404) {
      this.errorMessage = this.TextStringsUtil.DialogWeatherProviderLocationSaveLocationNotFoundFailedMessage;
    } else {
      this.errorMessage = this.TextStringsUtil.DialogWeatherProviderLocationSaveFailedMessage;
    }
  }
}
