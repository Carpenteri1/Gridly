import { Injectable, inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { IconModel } from '../../Models/icon.Model';
import { Observable } from 'rxjs/internal/Observable';
import { take } from 'rxjs/internal/operators/take';
import { UrlStringsUtil } from '../../Constants/url.strings.util';
import { SearchIconsResultDto } from '../../DTOs/SearchIconsResultDto';

@Injectable({
  providedIn: 'root'
})
export class IconEndpointService{
  private http = inject(HttpClient);


  get(): Observable<IconModel> {
    return this.http.get<IconModel>(UrlStringsUtil.IconGet).pipe(take(1));
  }

  search(input: string): Observable<SearchIconsResultDto> {
    return this.http.get<SearchIconsResultDto>(UrlStringsUtil.IconUrlSearch+input).pipe(take(1));
  }
}