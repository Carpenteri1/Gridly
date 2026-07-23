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

  get(): Observable<WeatherModel> {
    //TODO SearchTerm will be added
    const params = new HttpParams().set('SearchTerm', 'The SearchTerm');
    return this.http.get<WeatherModel>(UrlStringsUtil.GetWeatherUrl,{params}).pipe(take(1));
  }
}
