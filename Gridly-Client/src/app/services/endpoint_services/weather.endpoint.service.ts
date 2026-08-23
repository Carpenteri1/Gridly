import { Injectable, inject } from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {urlConstants} from "../../constants/url.constants";
import {Observable, take} from "rxjs";
import {WeatherDataModel} from "../../models/weatherData.Model";
import {StoredWeatherDataModel} from "../../models/storedWeatherData.Model";

@Injectable({
  providedIn: 'root'
})

export class WeatherEndpointService{
  private http = inject(HttpClient);

  get(address: string): Observable<WeatherDataModel> {
    const params = new HttpParams().set('Address', address);
    return this.http.get<WeatherDataModel>(urlConstants.weather.get,{params}).pipe(take(1));
  }
  getStoredWeatherData(): Observable<StoredWeatherDataModel[]> {
    return this.http.get<StoredWeatherDataModel[]>(urlConstants.weather.getStoredWeatherData).pipe(take(1));
  }
  getvisualcrossingdata(address: string): Observable<WeatherDataModel> {
    const params = new HttpParams().set('Address', address);
    return this.http.get<WeatherDataModel>(urlConstants.weather.getVisualCrossingData,{params}).pipe(take(1));
  }
  save(weather: WeatherDataModel, cardId: number){
    return this.http.post(urlConstants.weather.save, {weather, cardId}).pipe(take(1));
  }
}
