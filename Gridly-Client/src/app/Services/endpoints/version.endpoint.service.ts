import { Injectable, inject } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {VersionModel} from "../../models/version.Model";
import {UrlStringsUtil} from "../../constants/url.strings.util";
import {Observable, take} from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class VersionEndpointService{
  private http = inject(HttpClient);


  get(): Observable<VersionModel> {
    return this.http.get<VersionModel>(UrlStringsUtil.GetVersionUrl).pipe(take(1));
  }
}
