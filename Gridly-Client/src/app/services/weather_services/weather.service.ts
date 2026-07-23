import {inject, Injectable, Signal} from "@angular/core";
import { firstValueFrom, Observable, Subject} from "rxjs";
import { toSignal } from "@angular/core/rxjs-interop";
import {WeatherModel} from "../../models/weather.Model";
import {WeatherEndpointService} from "../endpoint_services/weather.endpoint.service";

@Injectable({providedIn: 'root'})
export class WeatherService {
  private readonly weatherSubject = new Subject<WeatherModel>();
  private readonly weather$ = new Observable<WeatherModel>();
  readonly weather!: Signal<WeatherModel | undefined>;
  #api = inject(WeatherEndpointService);

  constructor() {
    this.weather$ = this.weatherSubject.asObservable();
    this.weather = toSignal(this.weather$);
  }

    private getWeather$ = () => this.#api.get();
    getWeather = () => firstValueFrom(this.getWeather$());
}
