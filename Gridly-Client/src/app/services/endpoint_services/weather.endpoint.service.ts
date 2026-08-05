import { Injectable, inject } from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {urlConstants} from "../../constants/url.constants";
import {Observable, take} from "rxjs";
import {WeatherModel} from "../../models/weather.Model";
import {WeatherDataDto} from "../../dtos/weatherDataDto";

@Injectable({
  providedIn: 'root'
})

export class WeatherEndpointService{
  private http = inject(HttpClient);

  get(location: string): Observable<WeatherModel> {
    const params = new HttpParams().set('SearchTerm', location);
    return this.http.get<WeatherModel>(urlConstants.weather.get,{params}).pipe(take(1));
  }
  getvisualcrossingdata(location: string): Observable<WeatherModel> {
    const params = new HttpParams().set('SearchTerm', location);
    return this.http.get<WeatherModel>(urlConstants.weather.getVisualCrossingData,{params}).pipe(take(1));
  }
  save(weather: WeatherDataDto){
    return this.http.post(urlConstants.weather.save, {weather}).pipe(take(1));
  }
}
