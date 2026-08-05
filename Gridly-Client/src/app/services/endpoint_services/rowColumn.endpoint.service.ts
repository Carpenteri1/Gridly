import { Injectable, inject } from '@angular/core';
import {LocaleStringsService} from "../locale_services/locale-strings.service";
import {Observable, take} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {RowColumnModel} from "../../models/rowColumn.Model";

@Injectable({
  providedIn: 'root'
})

export class RowColumnEndpointService{
  private http = inject(HttpClient);
  private localeStrings = inject(LocaleStringsService);

  get(): Observable<RowColumnModel[] | null> {
    return this.http.get<RowColumnModel[] | null>(this.localeStrings.locale.rowColumn.url.get).pipe(take(1));
  }
  batchSave(rows: RowColumnModel[]): Observable<RowColumnModel[]> {
    return this.http.post<RowColumnModel[]>(this.localeStrings.locale.rowColumn.url.batchSave, rows).pipe(take(1));
  }
}
