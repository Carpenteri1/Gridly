import {inject, Injectable, Signal} from "@angular/core";
import { firstValueFrom, Observable, Subject} from "rxjs";
import { toSignal } from "@angular/core/rxjs-interop";
import { HttpErrorResponse } from "@angular/common/http";
import {WeatherModel} from "../../models/weather.Model";
import {WeatherEndpointService} from "../endpoint_services/weather.endpoint.service";
import {ProviderKeysService} from "../provider_key_services/provider-keys.service";

@Injectable({providedIn: 'root'})
export class WeatherService {
  private readonly weatherSubject = new Subject<WeatherModel>();
  private readonly weather$ = new Observable<WeatherModel>();
  readonly weather!: Signal<WeatherModel | undefined>;
  #api = inject(WeatherEndpointService);
  #providerKeyService= inject(ProviderKeysService);

  constructor() {
    this.weather$ = this.weatherSubject.asObservable();
    this.weather = toSignal(this.weather$);
  }

    private getWeather$ = (location: string) => this.#api.get(location);
    public getWeather = (location: string) => firstValueFrom(this.getWeather$(location));
    private getVisualCrossingData$ = (location: string) => this.#api.getvisualcrossingdata(location);

    async getVisualCrossingData(location: string): Promise<WeatherModel | undefined> {
      try {
        const weather = await firstValueFrom(this.getVisualCrossingData$(location));
        this.weatherSubject.next(weather);
        return weather;
      } catch (error) {
        if (error instanceof HttpErrorResponse && (error.status === 401 || error.status === 412)) {
          this.#providerKeyService.promptForInvalidKey();
        }
        return undefined;
      }
    }
}
