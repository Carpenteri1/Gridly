import { Injectable, inject } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {VersionModel} from "../../models/version.Model";
import {urlConstants} from "../../constants/url.constants";
import {Observable, take} from "rxjs";

@Injectable({
  providedIn: 'root'
})

export class VersionEndpointService{
  private http = inject(HttpClient);

  get(): Observable<VersionModel> {
    return this.http.get<VersionModel>(urlConstants.version.get).pipe(take(1));
  }
}
