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
    return this.http.get<VersionModel>(UrlStringsUtil.VersionCurrentUrl).pipe(take(1));
  }

  GetLatestVersion(): Observable<VersionModel> {
    return this.http.get<VersionModel>(UrlStringsUtil.VersionLatestUrl).pipe(take(1));
  }

  StoreVersion(version: VersionModel): Observable<VersionModel> {
    return this.http.post<VersionModel>(UrlStringsUtil.VersionSaveUrl,version).pipe(take(1));
  }

}
