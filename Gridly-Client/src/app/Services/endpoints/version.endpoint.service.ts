import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {VersionModel} from "../../Models/Version.Model";
import {UrlStringsUtil} from "../../Constants/url.strings.util";
import {Observable, take} from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class VersionEndpointService{
  constructor(private http: HttpClient) {}

  GetVersion(): Observable<VersionModel> {
    return this.http.get<VersionModel>(UrlStringsUtil.GetVersionUrl).pipe(take(1));
  }
}
