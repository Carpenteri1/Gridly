import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { UrlStringsUtil } from "../../constants/url.strings.util";
import { Observable, take } from "rxjs";
import { ClockModel } from "../../models/clock.Model";

@Injectable({
  providedIn: 'root'
})
export class ClockEndpointService {
  private http = inject(HttpClient);

  get(timeZone: string): Observable<ClockModel> {
    const params = new HttpParams().set('SearchTerm', timeZone);
    return this.http.get<ClockModel>(UrlStringsUtil.GetClockUrl, { params }).pipe(take(1));
  }

  getTimeApiData(timeZone: string): Observable<ClockModel> {
    const params = new HttpParams().set('SearchTerm', timeZone);
    return this.http.get<ClockModel>(UrlStringsUtil.GetTimeApiDataUrl, { params }).pipe(take(1));
  }
}
