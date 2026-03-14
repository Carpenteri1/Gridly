import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import { IconModel } from '../../Models/Icon.Model';
import { Observable } from 'rxjs/internal/Observable';
import { take } from 'rxjs/internal/operators/take';
import { UrlStringsUtil } from '../../Constants/url.strings.util';
import { SearchIconsResultDto } from '../../DTOs/SearchIconsResultDto';

@Injectable({
  providedIn: 'root'
})
export class IconEndpointService{
    constructor(private http: HttpClient) {}

  get(): Observable<IconModel> {
    return this.http.get<IconModel>("api/Icon/get").pipe(take(1));
  }

  search(): Observable<SearchIconsResultDto> {
    return this.http.get<SearchIconsResultDto>("api/Icon/search").pipe(take(1));
  }
}