import { Injectable, inject } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {VersionModel} from "../../models/version.Model";
import {LocaleStringsService} from "../locale_services/locale-strings.service";
import {Observable, take} from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class VersionEndpointService{
  private http = inject(HttpClient);
  private localeStrings = inject(LocaleStringsService);

  get(): Observable<VersionModel> {
    return this.http.get<VersionModel>(this.localeStrings.locale.version.url.get).pipe(take(1));
  }
}
