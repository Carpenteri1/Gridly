import { Injectable, inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { IconModel } from '../../models/icon.Model';
import { Observable } from 'rxjs/internal/Observable';
import { take } from 'rxjs/internal/operators/take';
import { LocaleStringsService } from '../locale_services/locale-strings.service';
import { SearchIconsResultDto } from '../../dtos/searchIconsResultDto';

@Injectable({
  providedIn: 'root'
})
export class IconEndpointService{
  private http = inject(HttpClient);
  private localeStrings = inject(LocaleStringsService);


  get(): Observable<IconModel> {
    return this.http.get<IconModel>(this.localeStrings.locale.icon.url.get).pipe(take(1));
  }

  search(input: string): Observable<SearchIconsResultDto> {
    return this.http.get<SearchIconsResultDto>(this.localeStrings.locale.icon.url.search+input).pipe(take(1));
  }
}
