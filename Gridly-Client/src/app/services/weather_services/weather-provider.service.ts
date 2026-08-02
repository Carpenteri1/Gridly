import {inject, Injectable, Signal} from "@angular/core";
import { firstValueFrom, Observable, Subject} from "rxjs";
import { toSignal } from "@angular/core/rxjs-interop";
import { HttpErrorResponse } from "@angular/common/http";
import {WeatherModel} from "../../models/weather.Model";
import {WeatherEndpointService} from "../endpoint_services/weather.endpoint.service";
import {ProviderKeysService} from "../provider_key_services/provider-keys.service";

@Injectable({providedIn: 'root'})
export class WeatherProviderService {
  private readonly weatherSubject = new Subject<WeatherModel>();
  private readonly weather$ = new Observable<WeatherModel>();
  readonly weather!: Signal<WeatherModel | undefined>;
  #api = inject(WeatherEndpointService);
  #providerKeyService= inject(ProviderKeysService);

  constructor() {
    this.weather$ = this.weatherSubject.asObservable();
    this.weather = toSignal(this.weather$);
  }

    save = (weather: WeatherModel) => this.#api.save(weather);
    private getWeather$ = (location: string) => this.#api.get(location);
    private getVisualCrossingData$ = (location: string) => this.#api.getvisualcrossingdata(location);
    async getWeather(location: string): Promise<[weather: WeatherModel | undefined, status: number]> {
      let status!:number;
      try {
          const weather = await firstValueFrom(this.getWeather$(location));
          this.weatherSubject.next(weather);
          return [weather,200];
        } catch (error) {
          if (error instanceof HttpErrorResponse && (error.status === 401 || error.status === 412)) {
          this.#providerKeyService.promptForInvalidKey();
          status = error.status !== 412 ? 401 : 404;
        }
        return [undefined, status];
      }
    }
    async getVisualCrossingData(location: string): Promise<[weather: WeatherModel | undefined, status: number]> {
      let status!:number;
      try {
        const weather = await firstValueFrom(this.getVisualCrossingData$(location));
        this.weatherSubject.next(weather);
        return [weather,200];
      } catch (error) {
        if (error instanceof HttpErrorResponse && (error.status === 401 || error.status === 412)) {
          this.#providerKeyService.promptForInvalidKey();
          status = error.status !== 412 ? 401 : 404;
        }
        return [undefined, status];
      }
    }
}
