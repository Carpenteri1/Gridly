import {inject, Injectable, Signal} from "@angular/core";
import {BehaviorSubject, firstValueFrom, Observable, take} from "rxjs";
import { toSignal } from "@angular/core/rxjs-interop";
import { HttpErrorResponse } from "@angular/common/http";
import {WeatherEndpointService} from "../endpoint_services/weather.endpoint.service";
import {WeatherDataModel} from "../../models/weatherData.Model";
import {StoredWeatherDataModel} from "../../models/storedWeatherData.Model";

@Injectable({providedIn: 'root'})
export class WeatherProviderService {
  private readonly storedWeatherDataSubject =  new BehaviorSubject<StoredWeatherDataModel[]>([]);
  readonly storedWeatherData$: Observable<StoredWeatherDataModel[]>;
  readonly storedWeatherData!: Signal<StoredWeatherDataModel[]>;
  #api = inject(WeatherEndpointService);

  constructor() {
    this.storedWeatherData$ = this.storedWeatherDataSubject.asObservable();
    this.storedWeatherData = toSignal(this.storedWeatherData$, { initialValue: [] as StoredWeatherDataModel[] });
    this.refresh();
  }

    private save$ = (weather: WeatherDataModel, cardId: number) => this.#api.save(weather, cardId);
    private getWeather$ = (address: string) => this.#api.get(address);
    private getVisualCrossingData$ = (address: string) => this.#api.getvisualcrossingdata(address);


    save = async (weather: WeatherDataModel, cardId: number) => {
      await firstValueFrom(this.save$(weather, cardId));
      this.refresh();
    }


    refresh(): void {
      this.#api.getStoredWeatherData().pipe(take(1))
        .subscribe((storedWeatherData) =>
          this.storedWeatherDataSubject.next(storedWeatherData));
    }

  async getWeather(address: string): Promise<[weather: WeatherDataModel | undefined, status: number]> {
      try {
        const weather = await firstValueFrom(this.getWeather$(address));
        return [weather, 200];
      } catch (error) {
        if (error instanceof HttpErrorResponse) {
          return [undefined, error.status];
        }
        return [undefined, 0];
      }
    }
    async getVisualCrossingData(address: string): Promise<[weather: WeatherDataModel | undefined, status: number]> {
      try {
        const weather = await firstValueFrom(this.getVisualCrossingData$(address));
        return [weather, 200];
      } catch (error) {
        if (error instanceof HttpErrorResponse) {
          return [undefined, error.status];
        }
        return [undefined, 0];
      }
    }
}
