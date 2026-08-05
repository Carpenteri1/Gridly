import { Injectable, inject } from '@angular/core';
import {LocaleStringsService} from "../locale_services/locale-strings.service";
import {CardModel} from "../../models/card.Model";
import {Observable, take} from "rxjs";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})

export class CardEndpointService{
  private http = inject(HttpClient);
  private localeStrings = inject(LocaleStringsService);

  get(): Observable<CardModel[]> {
    return this.http.get<CardModel[]>(this.localeStrings.locale.card.url.get).pipe(take(1));
  }
}
