import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import { IconModel } from '../../Models/Icon.Model';
import { Observable } from 'rxjs/internal/Observable';
import { take } from 'rxjs/internal/operators/take';
import { UrlStringsUtil } from '../../Constants/url.strings.util';

@Injectable({
  providedIn: 'root'
})
export class IconEndpointService{
    constructor(private http: HttpClient) {}

  Get(): Observable<IconModel> {
    return this.http.get<IconModel>("api/Icon/Get").pipe(take(1));
  }
}