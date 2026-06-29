import { Injectable, inject } from '@angular/core';
import {UrlStringsUtil} from "../../constants/url.strings.util";
import {Observable, take} from "rxjs";
import {HttpClient} from "@angular/common/http";
import {RowColumnModel} from "../../models/rowColumn.Model";

@Injectable({
  providedIn: 'root'
})

export class RowColumnEndpointService{
  private http = inject(HttpClient);

  get(): Observable<RowColumnModel[]> {
    return this.http.get<RowColumnModel[]>(UrlStringsUtil.RowColumnUrlGet).pipe(take(1));
  }
  add(rowColumn: RowColumnModel): Observable<RowColumnModel> {
    return this.http.post<RowColumnModel>(UrlStringsUtil.RowColumnUrlSave, rowColumn).pipe(take(1));
  }
  batchEdit(cards: RowColumnModel[]): Observable<RowColumnModel[]> {
    return this.http.post<RowColumnModel[]>(UrlStringsUtil.RowColumnUrlBatchEdit, cards).pipe(take(1));
  }
}
