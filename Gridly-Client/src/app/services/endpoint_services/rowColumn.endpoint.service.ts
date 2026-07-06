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

  get(): Observable<RowColumnModel[] | null> {
    return this.http.get<RowColumnModel[] | null>(UrlStringsUtil.RowColumnUrlGet).pipe(take(1));
  }
  batchSave(rows: RowColumnModel[]): Observable<RowColumnModel[]> {
    return this.http.post<RowColumnModel[]>(UrlStringsUtil.RowColumnUrlBatchSave, rows).pipe(take(1));
  }
}
