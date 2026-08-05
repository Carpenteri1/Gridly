import { Injectable, inject } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { IconModel } from '../../models/icon.Model';
import { Observable } from 'rxjs/internal/Observable';
import { take } from 'rxjs/internal/operators/take';
import { urlConstants } from '../../constants/url.constants';
import { SearchIconsResultDto } from '../../dtos/searchIconsResultDto';

@Injectable({
  providedIn: 'root'
})
export class IconEndpointService{
  private http = inject(HttpClient);


  get(): Observable<IconModel> {
    return this.http.get<IconModel>(urlConstants.icon.get).pipe(take(1));
  }

  search(input: string): Observable<SearchIconsResultDto> {
    return this.http.get<SearchIconsResultDto>(urlConstants.icon.search+input).pipe(take(1));
  }
}
