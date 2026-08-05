import { Injectable, inject } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {LocaleStringsService} from "../locale_services/locale-strings.service";
import {Observable, take} from "rxjs";
import {Widget} from "../../interfaces/widget.Interface";

@Injectable({
  providedIn: 'root'
})

export class WidgetEndpointService{
  private http = inject(HttpClient);
  private localeStrings = inject(LocaleStringsService);
  get(): Observable<Widget[]> {
    return this.http.get<Widget[]>(this.localeStrings.locale.widget.url.get).pipe(take(1));
  }
}
