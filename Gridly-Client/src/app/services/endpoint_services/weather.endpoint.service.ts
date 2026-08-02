import { Injectable, inject } from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {UrlStringsUtil} from "../../constants/url.strings.util";
import {Observable, take} from "rxjs";
import {WeatherModel} from "../../models/weather.Model";

@Injectable({
  providedIn: 'root'
})

export class WeatherEndpointService{
  private http = inject(HttpClient);

  get(location: string): Observable<WeatherModel> {
    const params = new HttpParams().set('SearchTerm', location);
    return this.http.get<WeatherModel>(UrlStringsUtil.GetWeatherUrl,{params}).pipe(take(1));
  }
  getvisualcrossingdata(location: string): Observable<WeatherModel> {
    const params = new HttpParams().set('SearchTerm', location);
    return this.http.get<WeatherModel>(UrlStringsUtil.GetVisualCrossingDataURL,{params}).pipe(take(1));
  }
  save(weather: WeatherModel){
    this.http.post<WeatherModel>(UrlStringsUtil.SaveWeatherUrl,{weather}).pipe(take(1));
  }
}
