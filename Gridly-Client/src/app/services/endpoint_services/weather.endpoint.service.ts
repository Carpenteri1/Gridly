import { Injectable, inject } from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {LocaleStringsService} from "../locale_services/locale-strings.service";
import {Observable, take} from "rxjs";
import {WeatherModel} from "../../models/weather.Model";
import {WeatherDataDto} from "../../dtos/weatherDataDto";

@Injectable({
  providedIn: 'root'
})

export class WeatherEndpointService{
  private http = inject(HttpClient);
  private localeStrings = inject(LocaleStringsService);

  get(location: string): Observable<WeatherModel> {
    const params = new HttpParams().set('SearchTerm', location);
    return this.http.get<WeatherModel>(this.localeStrings.locale.weather.url.get,{params}).pipe(take(1));
  }
  getvisualcrossingdata(location: string): Observable<WeatherModel> {
    const params = new HttpParams().set('SearchTerm', location);
    return this.http.get<WeatherModel>(this.localeStrings.locale.weather.url.getVisualCrossingData,{params}).pipe(take(1));
  }
  save(weather: WeatherDataDto){
    return this.http.post(this.localeStrings.locale.weather.url.save, {weather}).pipe(take(1));
  }
}
